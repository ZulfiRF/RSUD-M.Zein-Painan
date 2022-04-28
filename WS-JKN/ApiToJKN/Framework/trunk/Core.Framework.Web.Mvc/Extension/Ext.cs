using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Core.Framework.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Framework.Web.Mvc.Extension
{
    /// <summary>
    /// Class Ext digunakan untuk Helper 
    /// </summary>
    public static class Ext
    {
        /// <summary>
        /// To the specified form. digunakan untuk merubah dari Form Collection ke bentuk Tabel Item
        /// </summary>
        /// <typeparam name="TEntity">Class dari Tabel Item</typeparam>
        /// <param name="form">Collection data yang akan di rubah</param>
        /// <returns>TEntity</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Add Item with List Item         
        /// <code>
        /// var form = new FormCollection();
        ///  var data = form.To<![CDATA[<Employe>]]>();
        ///</code>
        ///</example>
        public static TEntity To<TEntity>(this FormCollection form) where TEntity : TableItem
        {
            var obj = Activator.CreateInstance<TEntity>();
            var dictonary = form.AllKeys.ToDictionary<string, string, object>(allKey => allKey, allKey => ((form[allKey].Equals("undefined")) ? "" : form[allKey]));
            obj.OnInit(dictonary);

            return obj;
        }
        /// <summary>
        /// Jsons to object. digunakan untuk merubah dari json ke bentuk Tabel Item
        /// </summary>
        /// <typeparam name="TEntity">Class dari Tabel Item.</typeparam>
        /// <param name="jsonText">The json text.</param>
        /// <returns>TEntity</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Add Item with List Item         
        /// <code>
        /// var data =<![CDATA[{"status":"success","list":[{"ID":"00002","Name":"Ilham"}],"count":1}]]>;
        /// var obj= data.JsonToObject<![CDATA[<Employe>]]>();
        ///</code>
        ///</example>
        public static TEntity JsonToObject<TEntity>(this string jsonText) where TEntity : TableItem
        {
            var obj = Activator.CreateInstance<TEntity>();
            var dictonary = new Dictionary<string, object>();
            var jsonObject = JsonConvert.DeserializeObject(jsonText) as JObject;
            foreach (var property in obj.GetType().GetProperties(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                try
                {
                    if (jsonObject != null)
                    {
                        var propertyJson = jsonObject.SelectToken(property.Name);
                        if (propertyJson != null)
                        {
                            var jProperty = propertyJson as JValue;
                            if (jProperty != null)
                                dictonary.Add(property.Name, jProperty.Value);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            obj.OnInit(dictonary);
            return obj;
        }
        /// <summary>
        /// Jsons to list object. digunakan untuk merubah json ke bentuk list tabel Item
        /// </summary>
        /// <typeparam name="TEntity">Class dari Tabel Item.</typeparam>
        /// <param name="jsonObject">The json object.</param>
        /// <param name="propertyName">Child name dari json object</param>
        /// <returns>IEnumerable{TEntity}.</returns>
        /// <example> contoh yang digunakan untuk  memanggil  Add Item with List Item         
        /// <code>
        /// listModel = objectJson.JsonToListObject<![CDATA[<FamilyStructures>]]>("FamilyStructures");
        ///</code>
        ///</example>
        public static IEnumerable<TEntity> JsonToListObject<TEntity>(this JObject jsonObject, string propertyName) where TEntity : TableItem
        {
            var jObject = jsonObject.SelectToken(propertyName) as JObject;
            if (jObject != null)
                foreach (var family in jObject)
                {
                    var a = family.Value;
                    var tokens = a as IList<JToken>;
                    if (tokens != null)
                        foreach (var token in tokens)
                        {
                            var model = Activator.CreateInstance<TEntity>() as TableItem;
                            var dictonary = new Dictionary<string, object>();
                            foreach (var property in model.GetType().GetProperties(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
                            {
                                try
                                {
                                    if (token != null)
                                    {
                                        var propertyJson = token.SelectToken(property.Name);
                                        if (propertyJson != null)
                                        {
                                            var jProperty = propertyJson as JValue;
                                            if (jProperty != null)
                                                dictonary.Add(property.Name, jProperty.Value);
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            model.OnInit(dictonary);
                            if (dictonary.Count != 0)
                                yield return (TEntity)model;
                        }
                }
        }

    }
}