namespace DependencyLocation.Containers
{
  using System;
  using System.Diagnostics.Contracts;

  /// <summary>
  /// Tiene la responsabilidad de guardar una relación de una definición
  /// de tipo genérico con un tipo concreto que la implementa.
  /// </summary>
  internal class GenericDefinitionContainer
  {
    private readonly Type mGenericDefinition;
    private Type mConcreteGenericDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericDefinitionContainer"/> class.        
    /// </summary>
    /// <param name="genericDefiniton">The generic definiton.</param>
    public GenericDefinitionContainer(Type genericDefinition)
    {
      Contract.Requires(genericDefinition != null, "genericDefinition can't be null");
      if (!genericDefinition.IsGenericTypeDefinition)
      {
        throw new ArgumentException("genericDefinition must be a generic type definition");
      }

      this.mGenericDefinition = genericDefinition;
    }

    /// <summary>
    /// Sets the concrete type with which to create instances of the generic type definition.
    /// </summary>
    /// <param name="concrete">The concrete generic type definition.</param>
    /// <returns>Itself</returns>
    public GenericDefinitionContainer SetConcrete(Type concrete)
    {
      Contract.Requires(concrete != null, "Concrete type can't be null");
      Contract.Ensures(Contract.Result<GenericDefinitionContainer>().Equals(this));

      if (!concrete.IsGenericTypeDefinition)
      {
        throw new ArgumentException("Concrete type must NOT be a generic type definition");
      }

      if (concrete.IsAbstract)
      {
        throw new ArgumentException("Concrete type can't be abstract.");
      }

      this.mConcreteGenericDefinition = concrete;
      return this;
    }

    /// <summary>
    /// Determines whether this instance can make the specified generic type.
    /// </summary>
    /// <param name="generic">The generic type.</param>
    /// <returns>
    ///   <c>true</c> if this instance can make the specified generic type; otherwise, <c>false</c>.
    /// </returns>
    public bool CanMake(Type generic)
    {
      Contract.Requires(generic != null, "generic can't be null");
      return generic.IsGenericType
          && !generic.IsGenericTypeDefinition
          && generic.GetGenericTypeDefinition().Equals(this.GetGenericDefinition());
    }

    /// <summary>
    /// Makes the interface constructors.
    /// </summary>
    /// <param name="generic">The generic.</param>
    /// <returns>The interface constructors for the generic type</returns>
    public GenericDefinitionContainer AddInterfaceConstructors(Type generic, ConstructorContainer container)
    {
      Contract.Requires(generic != null, "generic can't be null");
      Contract.Requires(container != null, "The constructor container can't be null");
      Contract.Ensures(Contract.Result<GenericDefinitionContainer>().Equals(this));

      if (!this.CanMake(generic))
      {
        throw new ArgumentException("Cannot make the desired generic type.");
      }

      Type concreteGeneric = this.mConcreteGenericDefinition.MakeGenericType(generic.GetGenericArguments());
      container.AddInterfaceConstructors(generic, concreteGeneric);

      return this;
    }

    /// <summary>
    /// Gets the generic definition.
    /// </summary>
    /// <returns>The generic type definition</returns>
    public Type GetGenericDefinition()
    {
      Contract.Ensures(Contract.Result<Type>() != null);
      Contract.Ensures(Contract.Result<Type>().IsGenericTypeDefinition);
      return this.mGenericDefinition;
    }
  }
}
