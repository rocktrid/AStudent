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
using LINQtoCSV;
using System.Windows.Forms;
using System.IO;

/// <summary>
/// Необходимо припилить функцию изменения сервера БД для оперирования над данными. 
/// </summary>
namespace AStudent
{
    public partial class ReestrPg : Page
    {
        private readonly string OpenConnection = "Database = db_for_case_solution; Data Source = 127.0.0.1; User Id = root; Password = Password";
        private string NewQuery = "";
        private MySqlDataAdapter adapter;
        private DataTable DTable;
        List<string[]> ForDGrid;
        List<string> ForHeaders;
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
                System.Windows.MessageBox.Show("Подключение к БД реализовано");
                //Остальные StackPanel раскрываем, чтобы не раскрывать элементы внутри них, коих больше
                For_ActionBtns.Visibility = Visibility.Visible;
                WorkBenchStack.Visibility = Visibility.Visible;
                LowerActionPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("" + ex);
                System.Windows.MessageBox.Show("Ошибка подключения к БД");
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
                        System.Windows.MessageBox.Show("Не выбрана существующая позиция");
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
                        System.Windows.MessageBox.Show("Таблица выведена!");
                        BaseConn.BuildConnection.Close();
                    }
                }
                else System.Windows.MessageBox.Show("Ошибка формирования запроса: запрос на выборку пуст");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("" + ex);
                System.Windows.MessageBox.Show("Ошибка инициализации подключения, повторите снова или обратитесь к системному администратору");
            }
        }
        private void Action_4_Click(object sender, RoutedEventArgs e) //тип обновление update для БД
        {
            try
            {
                BaseConn.BuildConnection.Open();
                adapter.Update(DTable);
                System.Windows.MessageBox.Show("Successfull");
                BaseConn.BuildConnection.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("" + ex);
                System.Windows.MessageBox.Show("Ошибка инициализации записи в БД");
                BaseConn.BuildConnection.Close();
            }
        }

        private void BtnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            Workingbench.Columns.Clear();

            // диалоговое окно для открытия файла
            OpenFileDialog OpenFile = new OpenFileDialog
            {
                Filter = "CSV|*.csv"
            };
            DataTable DT = new DataTable();
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                For_ActionBtns.Visibility = Visibility.Visible;
                WorkBenchStack.Visibility = Visibility.Visible;
                LowerActionPanel.Visibility = Visibility.Visible;
                StreamReader sr = new StreamReader(OpenFile.FileName);
                ForDGrid = System.IO.File.ReadAllLines(OpenFile.FileName).Select(x => x.Split(';')).ToList(); // помещает в List построчно данные из csv по разделителю ';'
                //List<string[]> ForDGridClear = ForDGrid.ElementAt<string>(ForDGrid.Count).TrimStart(';'); // здесь должно было быть удаление ненужного символа, но что-то не так.
                ForHeaders = ForDGrid[0].ToList(); // для добавления заголовков
                foreach (var Header in ForHeaders)
                {
                    DT.Columns.Add(Header);
                }
                foreach (var x in ForDGrid.Skip(1))
                {
                    if (x.SequenceEqual(ForHeaders))   //LINQ проверка на наличие повторяющихся строки в двух созданных листах
                        continue;     //Пропуск строки с повторяющимися данными
                    DT.Rows.Add(x);
                }
            }
            // перенос данных в datagrid
            Workingbench.ItemsSource = DT.DefaultView;
        }

        private void BtnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            string CheckName = "";
            string savepath = string.Empty;
            SaveFileDialog SaveFile = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "NewDoc.csv"
            };
            if (SaveFile.ShowDialog() == DialogResult.OK)
            {
                savepath = System.IO.Path.GetFullPath(SaveFile.FileName);
                System.Windows.MessageBox.Show("Данные экспортируются в документ");
                if (File.Exists(CheckName))
                {
                    try
                    {
                        File.Delete(CheckName);
                        
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
                try
                {
                    List<string[]> ExpData;
                    //ExpData = Workingbench.Items;
                }
                catch { throw; }
                //int ColumnsCounter = Workingbench.Columns.Count();
                //string ColumnsNames = "";
                //DTable.Clear();
                //DTable = ((DataView)Workingbench.ItemsSource).ToTable();
                //string[] output = new string[(Convert.ToInt32(DTable.Rows))];
                //for (int i = 0; i < ColumnsCounter; i++)
                //{
                //    ColumnsNames += DTable.Columns[i].ColumnName.ToString() + ";";
                //}
                //output[0] += ColumnsNames;
                //for (int i = 1; (i - 1) < Convert.ToInt32(DTable.Rows); i++)
                //{
                //    for (int j = 0; j < ColumnsCounter; j++)
                //    {
                //        output[i] += DTable.Rows[i - 1].
                //    }
                //}
                //Add the Header row for CSV file.

                //здесь csv файл 

            }
        }
    }
}

