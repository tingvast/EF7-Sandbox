using DataAccess.Interaces;
using EF7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{

    public class TheDate : IThedata
    {
        public TheDate()
        {
            TheExpression = new List<Expression>();
        }
        public Type Subproperty
        {
            get; set;           
        }

        public string SubpropertyName { get; set; }

        public List<Expression> TheExpression
        {
            get; set;
        }
    }

    public static class PropertyProjectorFactory<T> where T : class, IEntity
    {
        public static IPropertyProjector<T> Create()
        {
            return new PropertyProjector<T>();
        }
    }


    public class PropertyProjector<T> : IPropertyProjector<T> , IIncludePropertySelector<T> where T : class, IEntity
    {

        public PropertyProjector()
        {
            TheData = new TheDate();

        }

        public IThedata TheData
        {
            get; private set;
        }


        public IIncludePropertySelector<T> Include<TProperty>(params Expression<Func<TProperty, dynamic>>[] p) where TProperty : class, IEntity
        {
            var type = typeof(TProperty);
            TheData.SubpropertyName = type.Name;

            TheData.Subproperty = type;

            //ParameterExpression peeee = Expression.Parameter(type, "p");
            //foreach (var hah in p)
            //{

            //    var body1111 = hah.Body as MemberExpression;
            //    var propertyInfo11111 = (PropertyInfo)body1111.Member;
            //    var nameOfTheProperty11 = propertyInfo11111.Name;
            //    var typeOfTheProperty11 = propertyInfo11111.PropertyType;

            //    var memberAccess1 = Expression.Property(peeee, nameOfTheProperty11);

            //    var genericSelectMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "Select").ToList()[0];
            //    //var selectMetgid = genericSelectMethod.MakeGenericMethod(type, typeOfSubProj);

            //    Type func1111 = typeof(List<>);
            //    Type generic2323311 = func1111.MakeGenericType(type);
            //    var newNavigat = Expression.New(generic2323311);

            //    var kalle = genericSelectMethod.Invoke(newNavigat, new object[] { });


            //    // p => new { ff = p.Location, ffff = p.Location1, fff = p.PreRegistrations.Select(pp => pp.Text) }
            //}



            TheData.TheExpression.AddRange(p);
            return this;
        }

        //public IPropertyProjector<T> Include<TProperty>(IPropertyProjector<TProperty> propertySelector)
        //{
        //    throw new NotImplementedException();
        //}

        public IPropertyProjector<T> Select(params Expression<Func<T, dynamic>>[] p1)
        {
            TheData.TheExpression.AddRange(p1);

            return this;
        }

        //public IPropertyProjector<T> SelectSimple(Expression<Func<T, dynamic>> f)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> Select<TProperty>(IPropertyProjector<TProperty> propertySelector, params Expression<Func<T, dynamic>>[] p1)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<TProperty> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> SelectNavigation<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> p, Expression<Func<TProperty, dynamic>> q)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> SelectSimple(Expression<Func<T, dynamic>> f)
        //{
        //    throw new NotImplementedException();
        //}

        //public IIncludePropertySelector<T> ThenInclude<TProperty>(params Expression<Func<TProperty, dynamic>>[] p)
        //{
        //    throw new NotImplementedException();
        //}

        //public IPropertyProjector<T> Where(params Expression<Func<T, dynamic>>[] p1)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
