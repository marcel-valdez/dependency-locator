/*
Requerimientos
--------------
Una lista de constructores tiene:
Información pública:
- interfaz que satisface
- parámetros con las que se puede crear
- nombre de implementación

información privada:
- clase que la implementa

Se comunica con: Contenedor de Constructores
Diseño Externo de la Interfaz:
*/

// Constructor calls:
ConstructorList list = new ConstructorList<IType>();
// or
ConstructorList list = new ConstructorList(typeof(IType));

// To set the concrete type
list.SetConcrete<Class>();
// or
list.SetConcrete(typeof(Class));

// To check if it satisfies the interface
bool isType = list.IsType<IType>();
//or
bool isType = list.IsType(typeof(IType));

// To check if it has a constructor with given parameters
bool hasConstructor = list.TryGetConstructor(out ctorInvoker, type_array as Type[]);
// or
bool hasConstructor = list.TryGetConstructor(out ctorInvoker, param_array as object[]);

/*
High Level Design
-----------------

*/

internal class InterfaceConstructor<IType> : InterfaceConstructors
{
	public InterfaceConstructor() : base(typeof(IType))
	{
	}
}

// Interface definition:
internal class InterfaceConstructors
{
	public InterfaceConstructors(Type interfaceType);
	
	// Sets the concrete implementation of the interface
	void SetConcrete<T>();
	// Sets the concrete implementation of the interface
	void SetConcrete(Type type);
	// Verifies if the interface is of type T
	bool IsType<T>();
	// Verifies if the interface is of type 'type'
	bool IsType(Type type);
	// Gets the interface for which the constructors are
	Type GetInterface();
	// Tries to get a constructor that can be called with the given parameter types
	bool TryGetConstructor(out ConstructorInvoker ctor, param Type[] parameterTypes);
	// Tries to get a constructor that can be called with the given parameters
	bool TryGetConstructor(out ConstructorInvoker ctor, param object[] parameters);
}