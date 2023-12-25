namespace Serenity.CodeGenerator;

public interface IEntityModelFactory
{
    EntityModel Create(IEntityModelInputs inputs);
}