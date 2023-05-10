namespace Serenity.CodeGenerator;

public interface IEntityModelGenerator
{
    EntityModel GenerateModel(IEntityModelInputs inputs);
}