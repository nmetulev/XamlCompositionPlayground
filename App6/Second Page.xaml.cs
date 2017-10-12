using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace App6
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Second_Page : Page
    {
        static List<Item> items;
        public Second_Page()
        {
            this.InitializeComponent();

            if (items == null)
            {
                items = new List<Item>();
                items.Add(new Item() { Name = "Test 1" });
                items.Add(new Item() { Name = "Test 2" });
                items.Add(new Item() { Name = "Test 3" });
                items.Add(new Item() { Name = "Test 4" });
                items.Add(new Item() { Name = "Test 5" });
                items.Add(new Item() { Name = "Test 6" });
                items.Add(new Item() { Name = "Test 7" });
                items.Add(new Item() { Name = "Test 8" });
                items.Add(new Item() { Name = "Test 9" });
                items.Add(new Item() { Name = "Test 10" });
                items.Add(new Item() { Name = "Test 11" });
                items.Add(new Item() { Name = "Test 12" });
                items.Add(new Item() { Name = "Test 13" });
                items.Add(new Item() { Name = "Test 14" });
                items.Add(new Item() { Name = "Test 15" });
                items.Add(new Item() { Name = "Test 16" });
                items.Add(new Item() { Name = "Test 17" });
                items.Add(new Item() { Name = "Test 18" });
            }

            listView.ItemsSource = items;
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //listView.PrepareConnectedAnimation("ItemAnimation", e.ClickedItem, "ItemThumbnail");
            Frame.Navigate(typeof(DetailsPage), e.ClickedItem);
        }
    }

    public class Item
    {
        public string Name { get; set; }
    }
}
