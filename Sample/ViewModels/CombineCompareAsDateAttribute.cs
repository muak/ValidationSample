using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Sample.ViewModels.ValidationAttributes
{
    public class CombineCompareAsDateAttribute : ValidationAttribute
    {
        Mode CompareOption;

        public CombineCompareAsDateAttribute(Mode compareOption)
        {
            this.CompareOption = compareOption;
        }

        public CombineCompareAsDateAttribute(Mode compareOption, string errorMessage) : this(compareOption)
        {
            this.CompareOption = compareOption;
            this.ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var v = value as IList;
            if (v == null)
                return ValidationResult.Success;    //想定外のは成功とする（他のルールに任せる）

            if (v.Count > 2) {
                return ValidationResult.Success;
            }

            var dateA = v[0] as DateTime?;
            var dateB = v[1] as DateTime?;

            if (dateA == null || dateB == null) {
                return ValidationResult.Success;
            }

            bool ret = true;
            switch (CompareOption) {
                case Mode.LessThan:
                    ret = dateA >= dateB;
                    break;
                case Mode.LessThanOrEqual:
                    ret = dateA > dateB;
                    break;
                case Mode.GreaterThan:
                    ret = dateA >= dateB;
                    break;
                case Mode.GreaterThanOrEqual:
                    ret = dateA > dateB;
                    break;
            }

            return ret ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }


    }

    public enum Mode
    {
        LessThan, LessThanOrEqual, GreaterThan, GreaterThanOrEqual,
    }
}
