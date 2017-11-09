using System;

namespace TasklistPresentation.Models
{
   public class GenericEventArgs<TValue> : EventArgs
   {
      private readonly TValue value;

      public GenericEventArgs(TValue value)
      {
         this.value = value;
      }

      public TValue Value
      {
         get { return value; }
      }
   }
}
