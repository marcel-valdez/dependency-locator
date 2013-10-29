namespace Impl.Test
{
  using Interfaces.Test;

  public class Service : IService
  {
    public int Serve(int number)
    {
      return number + 1;
    }
  }
}
