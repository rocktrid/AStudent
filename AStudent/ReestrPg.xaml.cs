using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using MySql.Data.MySqlClient;

/// <summary>
/// Необходимо припилить функцию изменения сервера БД для оперирования над данными. 
/// </summary>
namespace AStudent
{
    public partial class ReestrPg : Page
    {
        private string OpenConnection = "Database = db_for_case_solution; Data Source = 127.0.0.1; User Id = root; Password = Password";
        private string NewQuery = "";
        private MySqlDataAdapter adapter;
        private DataTable DTable;
        public ReestrPg()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Метод ниже осуществляет подключение для выбранного варианта действия (открытия той или иной таблицы)
        /// В зависимости от выбора осуществляется подключения к нужной таблице в БД
        /// </summary>
        public void ChooseConnection(string ConnectionToBase)
        {
            try
            {
                BaseConn.BuildConnection = new MySqlConnection(ConnectionToBase);
                BaseConn.BuildConnection.Open();
                MessageBox.Show("Подключение к БД реализовано");
                //Остальные StackPanel раскрываем, чтобы не раскрывать элементы внутри них, коих больше
                For_ActionBtns.Visibility = Visibility.Visible;
                WorkBenchStack.Visibility = Visibility.Visible;
                LowerActionPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                MessageBox.Show("Ошибка подключения к БД");
            }
        }
        private void CmbChooseTable_Selected(object sender, RoutedEventArgs e)
        {
            BtnLoad.IsEnabled = true;
        }
        /// <summary>
        /// Метод подгружает базу данных в соответствии с выбранным элементом из соответствующего ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int a = CmbChooseTable.SelectedIndex;
                switch (a)                         // в зависимости от выбора таблицы, будет сформирован запрос
                {
                    case 0:
                        NewQuery = "SELECT * FROM Students";

                        break;
                    case 1:
                        NewQuery = "SELECT * FROM Faculties";

                        break;
                    case 2:
                        NewQuery = "SELECT * FROM Studying_Groups";

                        break;
                    case 3:
                        NewQuery = "SELECT * FROM Partners_and_Organizations";

                        break;
                    default:
                        MessageBox.Show("Не выбрана существующая позиция");
                        break;
                }
                For_ActionBtns.Visibility = Visibility.Visible;
                WorkBenchStack.Visibility = Visibility.Visible;
                LowerActionPanel.Visibility = Visibility.Visible;
                ///
                /// Вывод таблицы из БД в DataGrid (В теории - нужно добыть IP)
                ///
                if (NewQuery != "")
                {
                    using (BaseConn.BuildConnection = new MySqlConnection(OpenConnection))
                    {
                        BaseConn.BuildConnection.Open();
                        MySqlCommand FillDGrid = new MySqlCommand(NewQuery, BaseConn.BuildConnection);                       
                        MySqlDataReader DReader = FillDGrid.ExecuteReader();// инициализирует чтение из БД
                       // если что, выше объявлены поля для adapter, datatable, newquery, openconnection;
                        adapter = new MySqlDataAdapter(NewQuery, BaseConn.BuildConnection); // адаптер для работы с записью (необязательно для записи здесь, можно снести в метод на клик
                        // но здесь он нужен для уточнения команд на апдейт удаление и добавление
                        DTable = new DataTable(); // для сохранения данных из БД во "временное хранилище"
                        DTable.Load(DReader); // подгружает в "хранилище" данные полученные из БД
                        MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter);
                        //ниже прописываются основные виды команд
                        adapter.InsertCommand = cb.GetInsertCommand();
                        adapter.UpdateCommand = cb.GetUpdateCommand();
                        adapter.DeleteCommand = cb.GetDeleteCommand();
                        Workingbench.AutoGenerateColumns = true; // подрубаем автогенерацию колонок, чтобы выводило те наименования, что в БД находятся
                        Workingbench.ItemsSource = DTable.DefaultView; // привязка данных из datatable  к datagrid
                        MessageBox.Show("Таблица выведена!");                        
                        BaseConn.BuildConnection.Close();
                    }
                }
                else MessageBox.Show("Ошибка формирования запроса: запрос на выборку пуст");
            }
            catch (Exception ex)
            {
                MessageBox.Show(""+ex);
                MessageBox.Show("Ошибка инициализации подключения, повторите снова или обратитесь к системному администратору");
            }
        }
        private void Action_4_Click(object sender, RoutedEventArgs e) //тип обновление update для БД
        {
            try
            {
                BaseConn.BuildConnection.Open();
                adapter.Update(DTable);
                MessageBox.Show("Successfull");
                BaseConn.BuildConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                MessageBox.Show("Ошибка инициализации записи в БД");
                BaseConn.BuildConnection.Close();
            }
        }
    }
}
