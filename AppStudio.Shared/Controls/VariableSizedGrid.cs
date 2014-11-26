using System;

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AppStudio.Controls
{
    public class VariableSizedGrid : GridView
    {
       private Int32 _positionOfItemAtArray = 0;

        protected override void OnItemsChanged(object element)
        {          
            base.OnItemsChanged(element);
        }

        protected override void PrepareContainerForItemOverride(
          DependencyObject element, object item)
        {
            int spanProperty = this.CalcuateSpanProperty();

            element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.ColumnSpanProperty, spanProperty);
            element.SetValue(Windows.UI.Xaml.Controls.VariableSizedWrapGrid.RowSpanProperty, spanProperty);

            this._positionOfItemAtArray++;
            base.PrepareContainerForItemOverride(element, item);
        }

        private int CalcuateSpanProperty()
        {
            return this._positionOfItemAtArray % 3 == 0 ? 2 : 1;
        }
    }
}
