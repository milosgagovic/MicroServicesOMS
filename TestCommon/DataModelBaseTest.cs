using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCommon
{
    public abstract class DataModelBaseTest
    {
        /// <summary>
        /// Global ID of entity which is tested.
        /// </summary>
        protected long entityGid;

        /// <summary>
        /// Local ID of entity which is tested.
        /// </summary>
        protected long entityLid;

        /// <summary>
        /// Entity which is tested and which is initialized in test set-up.
        /// </summary>
        protected IdentifiedObject entity;

        /// <summary>
        /// Generator of unique property values.
        /// </summary>
        protected PropertyGenerator generator;

        /// <summary>
        /// Instance of ModelResourcesDescs used for various operations with ModelCodes.
        /// </summary>
        protected ModelResourcesDesc modelResourcesDescs;

        /// <summary>
        /// Test fixture set-up method.
        /// </summary>
        public virtual void Init()
        {
            generator = new PropertyGenerator();
            modelResourcesDescs = new ModelResourcesDesc();
        }



        /// <summary>
        /// Test which should be called for all settable properties, 
        /// in order to verify functionality of IAccess methods.
        /// </summary>
        /// <param name="propertyId">property ID of settable property</param>
        public virtual void SettablePropertiesTest(ModelCode propertyId)
        {
            Assert.IsTrue(entity.HasProperty(propertyId));

            Property initialValue = generator.Generate(propertyId);
            Assert.NotNull(initialValue);

            entity.SetProperty(initialValue);
            Property currentValue = entity.GetProperty(propertyId);
            Assert.AreEqual(initialValue, currentValue);
        }

        /// <summary>
        /// Test which should be called for all not-settable properties, 
        /// in order to verify that modification via IAccess methods is not possible.
        /// </summary>
        /// <param name="propertyId">property ID of settable property</param>/*
        public virtual void NotSettablePropertiesTest(ModelCode propertyId)
        {
            Assert.IsTrue(entity.HasProperty(propertyId));

            Property newValue = new Property(propertyId);

            Assert.Throws<ModelException>(delegate { entity.SetProperty(newValue); });

        }

        #region Refactoring

        /// <summary>
        /// Test which should be called for all not-settable reference properties, 
        /// in order to verify that IReference methods for those are working properly.
        /// </summary>
        /// <param name="propertyId">property ID of non-settable reference property</param>
         /*public virtual void NotSettableReferencePropertiesTest(ModelCode propertyId)
          {
              bool checkReference;
              List<ModelCode> inverseProperties = null;
              List<DMSType> types;
              modelResourcesDescs.GetInverseAssociationProperty(true, 0, propertyId, out inverseProperties, out types, out checkReference, true);

              Assert.LessOrEqual(1, inverseProperties.Count);

              foreach (ModelCode inversePropertyId in inverseProperties)
              {
                  long testLid1 = ModelCodeHelper.CreateLocalId(5, (short)DMSType.SUBSTATION, 7);

                  Assert.AreEqual(0, entity.GetProperty(propertyId).AsReferences().Count);
                  Assert.IsFalse(entity.IsReferenced);

                  entity.AddReference(inversePropertyId, testLid1);

                  Assert.IsTrue(entity.CheckReference(inversePropertyId, testLid1));
                  Assert.IsTrue(entity.IsReferenced);

                  List<long> currentPropertyValue = entity.GetProperty(propertyId).AsReferences();
                  Assert.AreEqual(1, currentPropertyValue.Count);
                  Assert.AreEqual(testLid1, currentPropertyValue[0]);

                  long updatedTestLid1 = ModelCodeHelper.CreateLocalId(5, (short)DMSType.SUBSTATION, 8);

                  entity.UpdateReference(inversePropertyId, testLid1, updatedTestLid1);

                  Assert.IsFalse(entity.CheckReference(inversePropertyId, testLid1));
                  Assert.IsTrue(entity.CheckReference(inversePropertyId, updatedTestLid1));

                  currentPropertyValue = entity.GetProperty(propertyId).AsReferences();
                  Assert.AreEqual(1, currentPropertyValue.Count);
                  Assert.AreEqual(updatedTestLid1, currentPropertyValue[0]);

                  long testLid2 = ModelCodeHelper.CreateLocalId(5, (short)DMSType.SUBSTATION, 9);

                  entity.AddReference(inversePropertyId, testLid2);

                  Assert.IsTrue(entity.CheckReference(inversePropertyId, testLid2));
                  Assert.IsTrue(entity.IsReferenced);

                  currentPropertyValue = entity.GetProperty(propertyId).AsReferences();
                  Assert.AreEqual(2, currentPropertyValue.Count);
                  Assert.AreEqual(updatedTestLid1, currentPropertyValue[0]);
                  Assert.AreEqual(testLid2, currentPropertyValue[1]);

                  entity.RemoveReference(inversePropertyId, updatedTestLid1);
                  entity.RemoveReference(inversePropertyId, testLid2);

                  Assert.IsFalse(entity.IsReferenced);

                  currentPropertyValue = entity.GetProperty(propertyId).AsReferences();
                  Assert.AreEqual(0, entity.GetProperty(propertyId).AsReferences().Count);
              }
          }
*/
        /// <summary>
        /// Test which should be called for all settable reference properties, 
        /// in order to verify that IReference methods for those are working properly.
        /// </summary>
        /// <param name="propertyId">property ID of non-settable reference property</param>
        /// 
        public virtual void SettableReferencePropertiesTest(ModelCode propertyId)
        {
            Assert.IsTrue(entity.HasProperty(propertyId));
            Property initialValue = generator.Generate(propertyId);
            Assert.NotNull(initialValue);

            entity.SetProperty(initialValue);



            Property currentProp = entity.GetProperty(propertyId);

            Assert.AreEqual(initialValue, currentProp);


        }



        #endregion

    }
}
