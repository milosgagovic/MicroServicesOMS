using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCommon
{
    public class PropertyGenerator
    {
        /// <summary>
        /// Index used for generating value of last generated property;
        /// </summary>
        private long currentIndex;

        /// <summary>
        /// Initializes new instance of PropertyGenerator.
        /// </summary>
        public PropertyGenerator()
        {
            currentIndex = 0;
        }

        /// <summary>
        /// Generate unique property value for provided property ID.
        /// </summary>
        /// <param name="propertyId">ModelCode of desired property</param>
        /// <returns>unique value of property with provided ID</returns>
        public Property Generate(ModelCode propertyId)
        {
            ++currentIndex;
            PropertyType propertyType = ModelCodeHelper.ExtractPropertyTypeFromModelCode(propertyId);


            switch (propertyType)
            {
                case PropertyType.Bool:
                    return new Property(propertyId, (currentIndex % 2 == 1));

                case PropertyType.Byte:
                    return new Property(propertyId, (byte)(currentIndex % 254));

                case PropertyType.Float:
                    return new Property(propertyId, currentIndex + 0.1f);

                /*case PropertyType.Double:
                    return new Property(propertyId, currentIndex + 0.1);*/

                case PropertyType.String:
                    return new Property(propertyId, String.Format("Index{0}", currentIndex));

                case PropertyType.Enum:
                    return new Property(propertyId, (short)((currentIndex % 4) + 1));

                case PropertyType.Int32:
                    return new Property(propertyId, (int)(currentIndex % 10000));

                case PropertyType.Int64:
                    return new Property(propertyId, (int)(currentIndex % 100000));

                case PropertyType.Reference:
                    return new Property(propertyId, ModelCodeHelper.CreateGlobalId(16, (short)DMSType.TERMINAL, (int)(currentIndex % 1000)));
                // izmjena LID u GID
                case PropertyType.FloatVector:
                    List<float> floatValues = new List<float>(3);
                    for (int i = 0; i <= 2; i++)
                    {
                        floatValues.Add(currentIndex + 0.1f);
                        ++currentIndex;
                    }
                    return new Property(propertyId, floatValues);

                case PropertyType.StringVector:
                    List<string> stringValues = new List<string>(3);
                    for (int i = 0; i <= 2; i++)
                    {
                        stringValues.Add(String.Format("index{0}", currentIndex));
                        ++currentIndex;
                    }
                    return new Property(propertyId, stringValues);

                default:
                    throw new NotSupportedException(String.Format("Generating of properties of {0} type is not supported.", propertyType));
            }
        }
    }
}
