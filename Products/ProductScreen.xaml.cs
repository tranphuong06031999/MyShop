using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyShop.Products
{
    /// <summary>
    /// Interaction logic for ProductScreen.xaml
    /// </summary>
    public partial class ProductScreen : UserControl
    {
        class FilterEntity
        {
            public int Value { get; set; }
        }
        /// <summary>
        /// Class phân trang
        /// </summary>
        public class PagingInfo
        {
            public int RowsPerPage { get; set; }//Hàng trên mỗi trang
            public int CurrentPage { get; set; }//Trang hiện tại
            public int TotalPages { get; set; }//Tổng số trang
            public int TotalItems { get; set; }//Tổng số mặt hàng
            public List<string> Pages
            {
                get
                {
                    var result = new List<string>();

                    for (var i = 1; i <= TotalPages; i++)
                    {
                        result.Add($"{i} / {TotalPages}");
                        //result.Add(i);
                    }

                    return result;
                }
            }
        }

        PagingInfo _pagingInfo;
        FilterEntity _filterInfo;

        int rowsPerPage = 10;

        public ProductScreen()
        {
            InitializeComponent();
        }
        private async void UserControl_Initialized(object sender, EventArgs e)
        {
            progressBar.IsIndeterminate = true;
            statusLabel.Content = "Application is ready";

            _filterInfo = new FilterEntity() { Value = 8000000 };

            var db = new MyShopEntities();


            await Task.Run(() =>
            {
                Thread.Sleep(3000);
            });
            progressBar.IsIndeterminate = false;

            categoriesComboBox.ItemsSource = db.Categories.ToList();
            categoriesComboBox.SelectedIndex = 0;
            filterPanel.DataContext = _filterInfo;


        }


        /// <summary>
        /// Quay lại màn hình home
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backWard_Click(object sender, RoutedEventArgs e)
        {
            homeProduct.Children.Clear();
            homeProduct.Children.Add(new HomeScreen());
        }
        /// <summary>
        /// Giá trị của comboBox thay đổi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void categoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }

        #region Xử lý hiệu ứng Comboxbox
        /// <summary>
        /// Hiệu ứng khi chọn
        /// </summary>
        private void ComboProductTypes_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.LightGray;
        }

        /// <summary>
        /// Hiệu ứng khi bỏ chọn
        /// </summary>
        private void ComboProductTypes_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Background = Brushes.Transparent;
        }
        #endregion


        /// <summary>
        /// Button hamburger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void filterToggle_Click(object sender, RoutedEventArgs e)
        {
            if (filterPanel.IsCollapsed())
            {
                filterPanel.Show();
            }
            else
            {
                filterPanel.Collapse();
            }
        }

        private void priceRangeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();
        }
        private void addCategory_MouseEnter(object sender, MouseButtonEventArgs e)
        {

        }

        private void editCategory_MouseEnter(object sender, MouseButtonEventArgs e)
        {

        }

        private void deleteCategory_MouseEnter(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// Nhân giá trị text trong khung Search
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CalculatePagingInfo();
            UpdateProductView();

            statusLabel.Content = "Search";
        }

        private void productsListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            statusLabel.Content = "Detail product";
            var db = new MyShopEntities();
            dynamic itemProduct = (sender as ListView).SelectedItem;

            var product = db.Products.Find(itemProduct.Product_Id);

            var screen = new DetailProductScreen(product);
            screen.RefreshProductList = refresh;
            homeProduct.Children.Add(screen);

        }
        /// <summary>
        /// Giá trị trong comboBox thay đổi, Update lại ProductsView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboPageIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int nextPage = comboBoxPaging.SelectedIndex + 1;
            _pagingInfo.CurrentPage = nextPage;

            UpdateProductView();
        }
        /// <summary>
        /// Trang trước trang hiện tại
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousPaging_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = comboBoxPaging.SelectedIndex;
            if (currentIndex > 0)
            {
                comboBoxPaging.SelectedIndex--;
            }
        }
        /// <summary>
        /// Trang kế tiếp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPaging_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = comboBoxPaging.SelectedIndex;
            if (currentIndex <= _pagingInfo.Pages.Count)
            {
                comboBoxPaging.SelectedIndex++;
            }
        }
        /// <summary>
        /// Tính toán số trang của một category
        /// </summary>
        void CalculatePagingInfo()
        {
            var db = new MyShopEntities();

            var _selectedCategoryIndex = categoriesComboBox.SelectedItem as Category;
            var products = db.Categories.Find(_selectedCategoryIndex.Category_Id).Products;
            var keyword = searchTextBox.Text;

            var query = from product in products
                        where product.Product_Name.ToLower()
                                .Contains(keyword.ToLower())
                        select product.Product_Id;

                        //&& product.Price <= _filterInfo.Value
                        //select new
                        //{
                        //    product.Product_Id,
                        //    ProductName = product.Product_Name,
                        //    Thumbnail = product.Photos.First().Data,
                        //    product.Price
                        //};

            // Tinh toan thong tin phan trang
            var count = query.Count();
            _pagingInfo = new PagingInfo()
            {
                RowsPerPage = rowsPerPage,
                TotalItems = count,
                TotalPages = count / rowsPerPage +
                    (((count % rowsPerPage) == 0) ? 0 : 1),
                CurrentPage = 1
            };

            comboBoxPaging.ItemsSource = _pagingInfo.Pages;
            comboBoxPaging.SelectedIndex = 0;

            statusLabel.Content = $"Tổng sản phẩm: {count} ";

        }
        /// <summary>
        /// Cập nhật lại danh sách sản phẩm
        /// </summary>
        void UpdateProductView()
        {
            var db = new MyShopEntities();

            var _selectedCategoryIndex = categoriesComboBox.SelectedItem as Category;
            var products = db.Categories.Find(_selectedCategoryIndex.Category_Id).Products;
            var keyword = searchTextBox.Text;

            var query = from product in products
                        where product.Product_Name.ToLower()
                                          .Contains(keyword.ToLower())
                                          //&& product.Price <= _filterInfo.Value
                        select new
                        {
                            product.Product_Id,
                            ProductName = product.Product_Name,
                            Thumbnail = product.Photos.First().Data,
                            product.Price
                        };

            
            // Gan du lieu cho list view de o cuoi cung
            // Dua theo trang hien tai
            var skip = (_pagingInfo.CurrentPage - 1) * _pagingInfo.RowsPerPage;
            var take = _pagingInfo.RowsPerPage;
            productsListView.ItemsSource = query.Skip(skip).Take(take);
        }

        private void AddProductItem_Click(object sender, RoutedEventArgs e)
        {
            var screen = new NewProductScreen();
            screen.RefreshProductList = refresh;
            homeProduct.Children.Add(screen);
        }

        private void EditProductItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteProductItem_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Làm mới danh sách sản phẩm (list view)
        /// </summary>
        public void refresh(bool Data)
        {
            if (!Data) return;

            CalculatePagingInfo();
            UpdateProductView();
        }

    }

}
