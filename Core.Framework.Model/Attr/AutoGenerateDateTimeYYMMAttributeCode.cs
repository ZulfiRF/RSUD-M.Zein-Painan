﻿namespace Core.Framework.Model.Attr
{
    public class AutoGenerateDateTimeYYMMAttributeCode : SkipAttribute
    {
        #region TypeAutoGenerate enum

        /// <summary>
        ///     Enum TypeAutoGenerate
        /// </summary>
        public enum TypeAutoGenerate
        {
            /// <summary>
            ///     The auto fill
            /// </summary>
            AutoFill,

            /// <summary>
            ///     The last index
            /// </summary>
            LastIndex,
            CanGenerateManual
        }

        #endregion TypeAutoGenerate enum
        /// <summary>
        ///  Initializes a new instance of the <see cref="AutoGenerateDateTimeYYMMAttributeCode" /> class.
        /// </summary>
        /// <param name="length">berisikan panjang karakter yang akan di generate</param>
        /// <param name="code">Berisikan Code yang akan di generate</param>
        public AutoGenerateDateTimeYYMMAttributeCode(int length, string code)
        : this(length, code, TypeAutoGenerate.LastIndex)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoGenerateDateTimeYYMMAttributeCode" /> class.
        /// </summary>
        /// <param name="length">berisikan panjang karakter yang akan di generate</param>
        /// <param name="code">Berisikan Code yang akan di generate</param>
        /// <param name="typeAutoGenerate">The type auto generate.</param>
        public AutoGenerateDateTimeYYMMAttributeCode(int length, string code, TypeAutoGenerate typeAutoGenerate)
        {
            Length = length;
            Code = code;
            TypeGenerate = typeAutoGenerate;
        }
        /// <summary>
        ///     Gets the length.
        /// </summary>
        /// <value>berisikan panjang karakter yang akan di generate</value>
        public int Length { get; private set; }

        /// <summary>
        /// Gets the Code   
        /// </summary>
        /// <value>Berisikan Code yang akan di generate</value>
        public string Code { get; private set; }
        /// <summary>
        ///     Gets or sets the property.
        /// </summary>
        /// <value>The property.</value>
        public string Property { get; set; }

        /// <summary>
        ///     Gets the type generate.
        /// </summary>
        /// <value>The type generate.</value>
        public TypeAutoGenerate TypeGenerate { get; private set; }
    }
}