using FluentValidation.Validators;

namespace Base.Application.Common.Validation;

public static class SetNonNullableValidatorExtension
{
    public static IRuleBuilderOptions<T, TProperty?> SetNonNullableValidator<T, TProperty>(
        this IRuleBuilder<T, TProperty?> ruleBuilder, IValidator<TProperty> validator, params string[] ruleSets)
    {
        var adapter = new NullableChildValidatorAdapter<T, TProperty>(validator, validator.GetType())
        {
            RuleSets = ruleSets
        };

        return ruleBuilder.SetAsyncValidator(adapter);
    }

    private class NullableChildValidatorAdapter<T, TProperty> : ChildValidatorAdaptor<T, TProperty>,
        IPropertyValidator<T, TProperty?>, IAsyncPropertyValidator<T, TProperty?>
    {
        public NullableChildValidatorAdapter(IValidator<TProperty> validator, Type validatorType) : base(validator,
            validatorType)
        {
        }

        public NullableChildValidatorAdapter(
            Func<ValidationContext<T>, TProperty, IValidator<TProperty>> validatorProvider, Type validatorType) : base(
            validatorProvider, validatorType)
        {
        }

#pragma warning disable RCS1132
        public override bool IsValid(ValidationContext<T> context, TProperty? value)
        {
            return base.IsValid(context, value!);
        }

        public override Task<bool> IsValidAsync(ValidationContext<T> context, TProperty? value,
            CancellationToken cancellationToken)
        {
            return base.IsValidAsync(context, value!, cancellationToken);
        }
#pragma warning restore RCS1132
    }
}