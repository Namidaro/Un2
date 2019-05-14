﻿using System.Collections.Generic;

namespace Unicon2.Infrastructure.Common
{
   public class Maybe<T>
   {
       private List<T> _innerList=new List<T>();

       public void Reset()
       {
           _innerList=new List<T>();
       }

       public void AddValue(T value)
       {
           _innerList.Add(value);
       }

       public void AddValueRange(IEnumerable<T> values)
       {
           _innerList.AddRange(values);
       }

       public List<T> GetListOfValue()
       {
           return _innerList;
       }
        
       public T GetFirstValue()
       {
          return _innerList[0];
       }

       public bool Any()
       {
           return _innerList.Count != 0;
       }

   }
}
