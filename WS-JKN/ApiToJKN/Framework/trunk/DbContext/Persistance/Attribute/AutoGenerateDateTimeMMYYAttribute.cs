using System;

namespace DbContext.Persistance
{
    public class AutoGenerateDateTimeYYMMAttribute : CustomAttribute
    {
        public enum TypeAutoGenerate
        {
            AutoFill,
            LastIndex
        }
        public int Length { get; private set; }
        public TypeAutoGenerate TypeGenerate { get; private set; }
        public string Property { get; set; }
        public AutoGenerateDateTimeYYMMAttribute(int length)
            : this(length, TypeAutoGenerate.LastIndex)
        {
        }
        public AutoGenerateDateTimeYYMMAttribute(int length, TypeAutoGenerate typeAutoGenerate)
        {
            this.Length = length;
            this.TypeGenerate = typeAutoGenerate;
        }
    }
}
