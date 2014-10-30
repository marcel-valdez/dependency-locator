<h2>What is Dependency Locator?</h2>
Dependency Locator is an implementation of the Service Locator pattern with a twist (aren't they all?). 
Dependency Locator uses a C# API for setting up dependencies (no more hundreds of XML configuration for your dependencies).

<h2>Features</h2>
<ul>
<li>Fluent C# API for Dependency configuration</li>
<li>It can setup Singletons</li>
<li>It can setup Configuration values</li>
<li>It can setup Lazy evaluated Dependencies, Singletons or Configuration values</li>
<li>It can setup Named dependencies</li>
<li>It understands generic interfaces and implementations</li>
</ul>

<a href="https://www.ohloh.net/p/dependency-locator?ref=sample" target="_top">
<img alt="Ohloh project report for dependency-locator" border="0" src="https://www.ohloh.net/p/dependency-locator/widgets/project_partner_badge.gif">
</a>


<h2>What are its advantages?</h2>
<ul>
<li>Almost no XML configuration (the example given here is your template, you just have to change 1 line)</li>
<li>Minor uninvasive changes to existing code needed to refactor out dependencies</li>
<li>Can be used for integration testing, by setting up  a combination of dummy and real implementations under test</li>
<li>Promotes the use of modularity in the system structure</li>
<li>Its perfect for refactoring legacy code</li>
<li>Its perfect for making legacy code more testable</li>
</ul>

<h2>Story of Example Usage</h2>

*Note*: Removed all usings and code that is not pertinent to this example.

Let's say John has an application where you have a View, Model and Controller as such:

<b>The Account Controller<b>
````csharp
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
<b>The Account Details View</b>
````html
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
<b>The Account model</b>
````csharp
namespace Bank.Models {
	public class Account {
		public string AccountHolder { get; set; }
		public decimal Balance { get; private set; }
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
````
<b>or</b>
````csharp
model.Withdraw(9999);
````
The View has no need and should not use any of these methods. It is a mistake for it to have access to those methods, and should not be visible to the view.

So Dave notices this design smell, and wants to add a ViewModel, so the View uses a reduced interface of the Model, but he doesn't want to move around any of the model code because he is too lazy (in a good way!) to modify someone else's code.

But his Boss tells the team: The size of the ASP.NET MVC website project is getting too big and unwieldly, from now on, any new classes that aren't Views, Models or Controllers added to the project must be placed in separate C# library projects.

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

Dave procedes to add a reference to the ASP.Net MVC Website Project because there is where the model is, but when he tries to add a reference from the ASP.Net MVC Website project to the Bank.ViewModels, catastrophe happens! He cannot because a circular dependency would occur:
Bank.ViewModels -- depends -> Bank -- depends -> Bank.ViewModels

Aha! But Dave is a smart guy and he knows that in these cases he has to use the Inversion of Control principle, so he creates ANOTHER project, that will contain the public interfaces of the ViewModels project.

So he goes ahead and creates a new C# library project called Bank.ViewModel.Interfaces:
````csharp
namespace Bank.ViewModels.Interfaces {
	public interface IAccountDetailsViewModel {
		string AccountHolder { get; }
		int AccountNumber { get; }
		decimal Balance { get; }
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
Now, the controller has to provide the View with the <b>interface</b> of the ViewModel, but as we know, one cannot just "instantiate" an interface, it needs to call the constructor, *here* is where _DependencyLocator_ comes into play.

In order for Dave to setup this dependency, all he has to do are 2 things:

**First**. He has to tell the Dependency Locator it has how to connect an interface with an implementation. This is easily done by creating a config.dependfile.

Contents of config.dependfile:

````
@Using(path/to/Bank.ViewModels.dll)
@Using(path/to/Bank.ViewModels.Interfaces.dll)

using Bank.ViewModels;
using Bank.ViewModels.Intefaces;

Config.SetupDependency<IAccountDetailsViewModel, AccountDetailsViewModel>();
````

The **.dependfile** could be in charge of setting up N interface libraries with M implementation libraries. In such a case it would just have references to all the interface and implementation libraries it sets up.

**Second**. Change the Global.asax.cs contents, the ScriptLoader will load the script and setup the dependencies:

````csharp
namespace Bank {
  public class BankMvcApplication : System.Web.HttpApplication {
    protected void Application_Start() {
      ScriptLoader.Load("path/to/config.dependfile");
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

Taddah! Now Dave has refactored the system to use IoC without breaking the Boss' rules, nor modifying much of the original code, notice we just modified 2 lines of executable code, added 1 new line in Global.asax, and added 3 using statements.

That's the main advantage of DependencyLocator, it is not invasive at all, so its very useful for refactoring legacy code.
