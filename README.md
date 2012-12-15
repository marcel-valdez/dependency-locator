<h2>Story of Example Usage</h2>

*Note*: Removed all usings and code that is not pertinent to this example.

Let's say John has an application where you have a View, Model and Controller as such:

````csharp
/* The Account Controller */
namespace Bank.Controllers {    
    using Bank.Models;
	
    public class AccountController : Controller {
        private readonly DataContext db = new DataContext();

        public ActionResult Details(int id) {
            Accounts account = db.Accounts.Find(id);

            if (account == null) {
                return HttpNotFound();
            }

            return View(account);
        }
		
		/* Rest of the code... */
    }
}
````

````html
/* The Account Details view*/
@model Bank.Model

@{
    ViewBag.Title = "My balance";
}

<h2>Account Details</h2>

<p>
	@Html.LabelFor(model => model.AccountHolder, "Account Holder: ");
	@Html.LabelFor(model => model.AccountNumber, "Account Number: ");
	@Html.LabelFor(model => model.Balance, "Balance: ");
</p>
````

````csharp
/* The Account model */
namespace Bank.Models {
	public class Account {
		public string AccountHolder { get; set; }
		
		public decimal Balance { get; }
		
		public int AccountNumber { get; set; }
		
		public void Deposit(decimal money) {
			if (money <= 0) throw new  Exception("Can't deposit less than or equal to $0.00");
			this.Balance += money;
		}
		
		public void Withdraw(decimal money) {
			if (money <= 0) throw new  Exception("Can't withdraw less than or equal to $0.00");
			this.Balance -= money;
		}
	}
}
````

You may have noticed that the View could call at any point in time:
````csharp
model.Deposit(9999); 
or
model.Withdraw(9999);
````
The View has no need and should not use any of these methods. It is a mistake for it to have access to those methods, and should not be visible to the view.

So Dave notices this design smell, and wants to add a ViewModel, so the View uses a reduced interface of the Model, but he doesn't want to move around any of the model code because he is too lazy (which is good!) to modify someone else's code.

But his Boss tells the team: The size the ASP.NET MVC website project is getting too big and unwieldly, from now on, any new classes that aren't Views, Models or Controller that are added to the project must be placed in separate C# library projects.

So Dave creates a new C# Library project called Bank.ViewModels and adds the following ViewModel:

````csharp
namespace Bank.ViewModels {
	using Bank.Models;
	
	public class AccountDetailsViewModel {
		public AccountDetailsViewModel(Account account) {
			this.AccountHolder = account.AccountHolder;
			this.AccountNumber = account.AccountNumber;
			this.Balance = account.Balance;
		}
		
		public string AccountHolder { get; private set; }
		
		public int AccountNumber { get; private set; }
		
		public decimal Balance { get; private set; }
	}
}
````

Dave procedes to add a reference to the ASP.Net MVC Website Project because there is where the model is, but when he tries to add a reference from the ASP.Net MVC Website project to the Bank.ViewModels, catastrophe! He cannot because a circular dependency would occur:
Bank.ViewModels depends on Bank and Bank would depend on Bank.ViewModels.

Aha! But Dave is a smart guy and he knows that in these cases he has to use the Inversion of Control principle, so he create ANOTHER project, that will contain the public interfaces of the ViewModels project.

So he goes ahead and creates a new C# library project called Bank.ViewModel.Interfaces:
````csharp
namespace Bank.ViewModels.Interfaces {
	public interface IAccountDetailsViewModel {
		public string AccountHolder { get; }
		public int AccountNumber { get; }
		public decimal Balance { get; private set; }
	}
}
````

Dave also procedes to change the definition of the AccountDetailsViewModel:

````csharp
namespace Bank.ViewModels {
	using Bank.Models;
	using Bank.ViewModels.Interfaces;
	
	public class AccountDetailsViewModel: IAccountDetailsViewModel {
		/* Rest of the code */
	}
}
````

So now, all Dave has to do is add a dependency on the ASP.Net Website to the Bank.ViewModels.Interfaces library, and change the View so it uses the ViewModel interface, and not the concrete implementation:

````csharp
@model Bank.ViewModels.Interfaces.IAccountDetailsViewModel

@{
    ViewBag.Title = "My balance";
}

<h2>Account Details</h2>

<p>
	@Html.LabelFor(model => model.AccountHolder, "Account Holder: ");
	@Html.LabelFor(model => model.AccountNumber, "Account Number: ");
	@Html.LabelFor(model => model.Balance, "Balance: ");
</p>
````
Now, the controller has to provide the View with the *interface* of the ViewModel, but as we know, one cannot just "instantiate" an interface, it needs to call the constructor, *here* is where DependencyLocator comes into play.

In order for Dave to setup this dependency, all he has to do are 2 things:
1. In the Bank.ViewModels project he has to add a new class for setting up the dependencies it provides as such:
````csharp
namespace Bank.ViewModels {
	using DependencyLocation;
	using DependencyLocation.Setup;
	using Bank.ViewModels.Intefaces;
	
	public class Setup : IDependencySetup {
	
		public void SetupDependencies(IDependencyConfigurator container, string prefix, string defaultKey) {
			container.SetupDependency<IAccountDetailsViewModel, AccountDetailsViewModel>(defaultKey);
		}
	}
}
````

2. He has to tell the Dependency Locator it has to load that Setup class. This is done by creating a dependency.config file and calling the Dependency Loader during startup in the Global.asax file.

Contents of dependency.config:
````xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <sectionGroup name="dependencyInjector">
      <section
        name="dependencyConfiguration"
        type="DependencyLocation.Configuration.DependencyConfiguration, DependencyLocator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        allowLocation="true"
        allowDefinition="Everywhere"
      />
    </sectionGroup>
  </configSections>

  <dependencyInjector>
    <dependencyConfiguration defaultKey="default">
      <dependencies>
        <add assemblyName="Bank.ViewModels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" namedInstancePrefix="" />
      </dependencies>
    </dependencyConfiguration>
  </dependencyInjector>
</configuration>
````
The assembly where the dependencies are setup doesn't necessarily have to be the same where the concrete implementations are provided. The assembly that sets up dependencies could be a separate one, and the setup Assembly could be in charge of setting N interface libraries and with M implementation libraries. In such a case the setup assembly would just have references to all the interface and implementation libraries it sets up.

Global.asax.cs contents, this will load any and all IDependencySetup implementors configured in the dependency.config file:
````csharp
namespace Bank {
    public class BankMvcApplication : System.Web.HttpApplication {
        protected void Application_Start() {
				DependencyLoader.Loader.LoadDependencies("dependency.config");
				
				/* Rest of the Application_Start code */
        }
		
		/* Rest of the Global.asax.cs file */
    }
}
````

Finally, Dave modifies the Controller code to return a ViewModel instead of a Model:
````csharp
namespace Bank.Controllers
{    
    using Bank.ViewModels.Interfaces;
	using Bank.Models;
	using DependencyLocation;
	
    public class AccountController : Controller {
        private readonly DataContext db = new DataContext();

        public ActionResult Details(int id) {
            Accounts account = db.Accounts.Find(id);

            if (account == null) {
                return HttpNotFound();
            }
		
			/* 
			* This will call the constructor for the concrete type setup fhr IAccountDetailsViewModel interface.
			*   Notice however, that it could just as well be a dummy implementation of the interface, useful for testing purposes.
			*/
            return View(Dependency.Locator.Create<IAccountDetailsViewModel>(account));
        }
		
		/* Rest of the code... */
    }
}
````