using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace CalculationApp
{
    /// <summary>
    /// Логика взаимодействия для CalculationOrdersWindow.xaml
    /// </summary>
    public partial class CalculationOrdersWindow : Window
    {
        CalculationEntities db = new CalculationEntities();
        Projects OrdersDatas = new Projects();
        AssignedTasks assignedTasks = new AssignedTasks();
        AssignedTasks assignedTasks1 = new AssignedTasks();
        AssignedTasks assignedTasks2 = new AssignedTasks();
        AssignedTasks assignedTasks3 = new AssignedTasks();
        List<Tasks> LstTasks = new List<Tasks>();
        List<Projects> ListOrders = new List<Projects>();
        Users user = new Users();

        public CalculationOrdersWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            OrdersDatas.IdProject = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
            StAutoOrdersDatas.DataContext = OrdersDatas;

            DateTime currentTime = DateTime.Today;
            TxtOrderAcceptanceDate.Text = currentTime.ToShortDateString();

            cmbService.ItemsSource = db.Services.ToList();
            LstTasks = db.Tasks.ToList();
            LvOrders.ItemsSource = db.Projects.OrderByDescending(rec=>rec.IdProject).ToList();

            cmbClient.Items.Add("Новый клиент");
            string[] clients = db.Users.Where(rec=>rec.IdRole==4).Select(p => p.Surname).Distinct().ToArray();
            foreach (string client in clients)
            {
                cmbClient.Items.Add(client);
            }
        }
        public void CmbSort()
        {
            if(cmbService.SelectedItem!=null)
            {
                int cont = (int)cmbService.SelectedValue;
                cmbTask1.ItemsSource = db.Tasks.Where(cc => cc.IdService == cont).ToList();

                var select1 = cmbTask1.SelectedItem as Tasks;
                if (select1 != null)
                {

                    var selectname1 = select1.Name;

                    string searchStrings1 = selectname1.Substring(0, 7);
                    var list = LstTasks.Where(n => n.Name.Contains(searchStrings1)).ToList();
                    foreach (var lst in list)
                    {
                        var zap = LstTasks.Where(l => l.IdTask == lst.IdTask).FirstOrDefault();
                        if (zap != null)
                        {
                            LstTasks.Remove(zap);
                        }
                    }
                    cmbTask2.ItemsSource = LstTasks.Where(cc => cc.IdService == cont);
                }
                var select2 = cmbTask2.SelectedItem as Tasks;
                if (select2 != null)
                {

                    var selectname2 = select2.Name;

                    string searchStrings2 = selectname2.Substring(0, 7);
                    var list = LstTasks.Where(n => n.Name.Contains(searchStrings2)).ToList();
                    foreach (var lst in list)
                    {
                        var zap = LstTasks.Where(l => l.IdTask == lst.IdTask).FirstOrDefault();
                        if (zap != null)
                        {
                            LstTasks.Remove(zap);
                        }
                    }
                    cmbTask3.ItemsSource = LstTasks.Where(cc => cc.IdService == cont);
                }
                btnCost.Visibility = Visibility.Visible;
            }
        }
        private void BtnLichnKabinet_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LichnKabinet lichnKabinet = new LichnKabinet();
            lichnKabinet.Show();
        }

        private void cmbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbService.SelectedValue != null)
            {
                TxtKolVoTasks.Visibility = Visibility.Visible;
                cmbKolVoTasks.Visibility = Visibility.Visible;
                CmbSort();
            }
            else
            {
                MessageBox.Show("Выберите услугу для расчёта");
            }
        }

        private void btnCost_Click(object sender, RoutedEventArgs e)
        {
            var task1 = (Tasks)cmbTask1.SelectedItem;
            var task2 = (Tasks)cmbTask2.SelectedItem;
            var task3 = (Tasks)cmbTask3.SelectedItem;
            if (cmbKolVoTasks.SelectedIndex == 0 && task1 !=null || cmbKolVoTasks.SelectedIndex == 1 && task1 != null && task2 != null ||
                cmbKolVoTasks.SelectedIndex == 2 && task1 != null && task2 != null && task3!=null)
            {
                if (task1 != null && task2 != null && task3 != null)
                {
                    if (DpStartDate.SelectedDate == null)
                    {
                        MessageBox.Show("Вы не выбрали дату начала!");
                    }
                    else
                    {
                        DateTime startdate = DpStartDate.SelectedDate.Value;
                        DateTime dates = startdate.AddDays(task1.Time + task2.Time + task3.Time + 3);
                        var costs = (task1.Cost + (task1.Cost / task1.Difficulties.PercentageOfCost)) + (task2.Cost + (task2.Cost / task2.Difficulties.PercentageOfCost)) + (task3.Cost + (task3.Cost / task3.Difficulties.PercentageOfCost));
                        TxtEndDate.Text = $"{dates.ToShortDateString()}";
                        TxtCost.Text = costs.ToString();
                    }
                }
                else if (task1 != null && task2 != null && task3 == null)
                {
                    if (DpStartDate.SelectedDate == null)
                    {
                        MessageBox.Show("Вы не выбрали дату начала!");
                    }
                    else
                    {
                        DateTime startdate = DpStartDate.SelectedDate.Value;
                        DateTime dates = startdate.AddDays(task1.Time + task2.Time + 3);
                        var costs = (task1.Cost + (task1.Cost / task1.Difficulties.PercentageOfCost)) + (task2.Cost + (task2.Cost / task2.Difficulties.PercentageOfCost));
                        TxtEndDate.Text = $"{dates.ToShortDateString()}";
                        TxtCost.Text = costs.ToString();
                    }
                }
                else if (task1 != null && task2 == null && task3 == null)
                {
                    if (DpStartDate.SelectedDate == null)
                    {
                        MessageBox.Show("Вы не выбрали дату начала!");
                    }
                    else
                    {
                        DateTime startdate = DpStartDate.SelectedDate.Value;
                        DateTime dates = startdate.AddDays(task1.Time + 3);
                        var costs = (task1.Cost + (task1.Cost / task1.Difficulties.PercentageOfCost));
                        TxtEndDate.Text = $"{dates.ToShortDateString()}";
                        TxtCost.Text = costs.ToString();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите все задачи, входящие в услугу!");
            }

        }

        private void cmbTask2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CmbSort();
        }

        private void cmbTask1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CmbSort();
        }

        private void BtnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            if (TxtIdOrder.Text!=null && TxtOrderAcceptanceDate.Text != null && TxtEndDate.Text != null && DpStartDate.SelectedDate != null && cmbService.SelectedValue != null && TxtCost.Text!=null && cmbClient.Text != null
                && cmbTask1.SelectedItem != null)
            {
                string client = cmbClient.SelectedValue.ToString();
                var idclient = db.Users.Where(rec => rec.Surname == client).Select(rec=>rec.IdUser).FirstOrDefault();

                OrdersDatas.IdProject = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
                OrdersDatas.IdService = (int)cmbService.SelectedValue;
                OrdersDatas.OrderAcceptanceDate = Convert.ToDateTime(TxtOrderAcceptanceDate.Text);
                OrdersDatas.StartDate = Convert.ToDateTime(DpStartDate.SelectedDate);
                OrdersDatas.EndDate = Convert.ToDateTime(TxtEndDate.Text);
                OrdersDatas.IdUser = idclient;
                OrdersDatas.Cost = Convert.ToDecimal(TxtCost.Text);

                var task1 = (Tasks)cmbTask1.SelectedItem;
                var task2 = (Tasks)cmbTask2.SelectedItem;
                var task3 = (Tasks)cmbTask3.SelectedItem;

                assignedTasks1.IdProject = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
                assignedTasks1.IdTask = task1.IdTask;
                assignedTasks1.AppointmentDate = Convert.ToDateTime(TxtOrderAcceptanceDate.Text);
                db.AssignedTasks.Add(assignedTasks1);

                if (cmbTask2.SelectedValue != null)
                {
                    assignedTasks2.IdProject = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
                    assignedTasks2.IdTask = task2.IdTask;
                    assignedTasks2.AppointmentDate = Convert.ToDateTime(TxtOrderAcceptanceDate.Text);

                    db.AssignedTasks.Add(assignedTasks2);
                }
                if (cmbTask3.SelectedValue != null)
                {
                    assignedTasks3.IdProject = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
                    assignedTasks3.IdTask = task3.IdTask;
                    assignedTasks3.AppointmentDate = Convert.ToDateTime(TxtOrderAcceptanceDate.Text);

                    db.AssignedTasks.Add(assignedTasks3);
                }
                db.Projects.Add(OrdersDatas);
                db.SaveChanges();
                MessageBox.Show("Вы успешно создали заказ");
                TxtEndDate.Text = "";
                DpStartDate.Text = "";
                cmbService.Text = "";
                TxtCost.Text = "";
                StTask1.Visibility = Visibility.Collapsed;
                StTask2.Visibility = Visibility.Collapsed;
                StTask3.Visibility = Visibility.Collapsed;
                StCost.Visibility = Visibility.Collapsed;
                TxtKolVoTasks.Visibility = Visibility.Collapsed;
                cmbKolVoTasks.Visibility = Visibility.Collapsed;
                cmbKolVoTasks.SelectedIndex = -1;
                cmbClient.SelectedIndex = -1;
                LvOrders.ItemsSource = db.Projects.ToList();
                LvTasks.ItemsSource = db.AssignedTasks.ToList();
            }
            else
            {
                MessageBox.Show("Введите необходимые данные!");
            }
        }

        private void BtnWord_Click(object sender, RoutedEventArgs e)
        {
            if (TxtIdOrder.Text != null && TxtOrderAcceptanceDate.Text != null && TxtEndDate.Text != null && DpStartDate.SelectedDate != null && cmbService.SelectedValue != null && TxtCost.Text != null)
            {
                    Word.Application wordApp = new Word.Application();
                    wordApp.Visible = true;
                string client = cmbClient.SelectedValue.ToString();
                var idclient = db.Users.Where(rec => rec.Surname == client).FirstOrDefault();

                var service = (Services)cmbService.SelectedItem;
                    DateTime startDate = DpStartDate.SelectedDate.Value;
                    var startdate = startDate.ToShortDateString();
                    var task1 = (Tasks)cmbTask1.SelectedItem;
                    var task2 = (Tasks)cmbTask2.SelectedItem;
                    var task3 = (Tasks)cmbTask3.SelectedItem;

                var id = db.Projects.OrderByDescending(rec => rec.IdProject).FirstOrDefault().IdProject + 1;
                // Создаем новый документ
                Word.Document doc = wordApp.Documents.Add();

                    // Заголовок документа
                    Word.Paragraph header = doc.Paragraphs.Add();
                    header.Range.Text = "Общество с ограниченной ответственностью «АртКлён»";
                    header.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                header.Range.Font.Name = "Times New Roman";
                header.Range.Font.Size = 14;
                header.Range.InsertParagraphAfter();

                    // Заголовок отчета
                    Word.Paragraph title = doc.Paragraphs.Add();
                    title.Range.Text = $"Договор на предоставление услуги по заказу № {id}";
                    title.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                title.Range.Font.Name = "Times New Roman";
                title.Range.Font.Size = 14;
                title.Range.InsertParagraphAfter();
                    title.Range.InsertParagraphAfter();

                    Word.Paragraph Utv = doc.Paragraphs.Add();
                    Utv.Range.Text = $"УТВЕРЖДАЮ";
                    Utv.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                Utv.Range.Font.Name = "Times New Roman";
                Utv.Range.Font.Size = 14;
                Utv.Range.InsertParagraphAfter();

                    Word.Paragraph Utv2 = doc.Paragraphs.Add();
                    Utv2.Range.Text = $"Главный бухгалтер";
                    Utv2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                Utv2.Range.Font.Name = "Times New Roman";
                Utv2.Range.Font.Size = 14;
                Utv2.Range.InsertParagraphAfter();

                    Word.Paragraph Utv3 = doc.Paragraphs.Add();
                    Utv3.Range.Text = $"ФИО";
                    Utv3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                Utv3.Range.Font.Name = "Times New Roman";
                Utv3.Range.Font.Size = 14;
                Utv3.Range.InsertParagraphAfter();

                    Word.Paragraph Utv4 = doc.Paragraphs.Add();
                    Utv4.Range.Text = $"{TxtOrderAcceptanceDate.Text} г.";
                    Utv4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                Utv4.Range.Font.Name = "Times New Roman";
                Utv4.Range.Font.Size = 14;
                Utv4.Range.InsertParagraphAfter();
                    Utv4.Range.InsertParagraphAfter();

                    Word.Paragraph Ab1 = doc.Paragraphs.Add();
                    Ab1.Range.Text = $"{idclient.Surname} {idclient.Name} {idclient.Patronymic}, именуемый в дальнейшем «Заказчик», действующего на основании Устава, " +
                        $"с одной стороны, и ООО «АртКлён», именуемый в дальнейшем «Исполнитель», " +
                        $"с другой стороны, именуемые в дальнейшем «Стороны», заключили настоящий Договор о нижеследующем:";
                    Ab1.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab1.Range.Font.Name = "Times New Roman";
                Ab1.Range.Font.Size = 14;
                Ab1.Range.InsertParagraphAfter();

                    Word.Paragraph Ab2 = doc.Paragraphs.Add();
                    Ab2.Range.Text = $"1. Предмет договора";
                    Ab2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                header.Range.Font.Name = "Times New Roman";
                header.Range.Font.Size = 14;
                Ab2.Range.InsertParagraphAfter();

                    Word.Paragraph Ab3 = doc.Paragraphs.Add();
                    Ab3.Range.Text = $"1.1. По договору возмездного оказания услуг " +
                        $"Исполнитель обязуется по заданию Заказчика оказать услуги, указанные в п.1.2 настоящего Договора, " +
                        $"а Заказчик обязуется принять и оплатить эти услуги.";
                    Ab3.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab3.Range.Font.Name = "Times New Roman";
                Ab3.Range.Font.Size = 14;
                Ab3.Range.InsertParagraphAfter();

                    Word.Paragraph Ab4 = doc.Paragraphs.Add();
                    Ab4.Range.Text = $"1.2. Исполнитель обязуется оказать следующие услуги: ";
                    Ab4.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab4.Range.Font.Name = "Times New Roman";
                Ab4.Range.Font.Size = 14;
                Ab4.Range.InsertParagraphAfter();

                    Word.Paragraph Ab5 = doc.Paragraphs.Add();
                    Ab5.Range.Text = $"Предоставляемая услуга: {service.Name}";
                    Ab5.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab5.Range.Font.Name = "Times New Roman";
                Ab5.Range.Font.Size = 14;
                Ab5.Range.InsertParagraphAfter();

                    Word.Paragraph Ab6 = doc.Paragraphs.Add();
                    Ab6.Range.Text = $"Таблица 1 – Список задач обозначенной услуги";
                    Ab6.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab6.Range.Font.Name = "Times New Roman";
                Ab6.Range.Font.Size = 14;
                Ab6.Range.InsertParagraphAfter();

                    Word.Table table2 = doc.Tables.Add(title.Range, 4, 4);
                    table2.Borders.Enable = 1;
                    table2.Cell(1, 1).Range.Text = $"№";
                    table2.Cell(1, 2).Range.Text = $"Наименование задачи";
                    table2.Cell(1, 3).Range.Text = $"Кол-во требуемого время, дн.";
                    table2.Cell(1, 4).Range.Text = $"Стоимость, руб.";
                table2.Range.Font.Name = "Times New Roman";
                table2.Range.Font.Size = 14;

                table2.Cell(2, 1).Range.Text = $"1";
                    table2.Cell(2, 2).Range.Text = $"{task1.Name}";
                    table2.Cell(2, 3).Range.Text = $"{task1.Time}";
                    table2.Cell(2, 4).Range.Text = $"{task1.Cost}";
                    if (task2 != null)
                    {
                        table2.Cell(3, 1).Range.Text = $"2";
                        table2.Cell(3, 2).Range.Text = $"{task2.Name}";
                        table2.Cell(3, 3).Range.Text = $"{task2.Time}";
                        table2.Cell(3, 4).Range.Text = $"{task2.Cost}";
                    }
                    if (task3 != null)
                    {
                        table2.Cell(4, 1).Range.Text = $"3";
                        table2.Cell(4, 2).Range.Text = $"{task3.Name}";
                        table2.Cell(4, 3).Range.Text = $"{task3.Time}";
                        table2.Cell(4, 4).Range.Text = $"{task3.Cost}";
                    }
                    Ab6.Range.InsertParagraphAfter();

                    Word.Paragraph Ab7 = doc.Paragraphs.Add();
                    Ab7.Range.Text = $"Дата начала исполнения обозначенной услуги: {startdate}.";
                    Ab7.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab7.Range.Font.Name = "Times New Roman";
                Ab7.Range.Font.Size = 14;
                Ab7.Range.InsertParagraphAfter();

                    Word.Paragraph Ab8 = doc.Paragraphs.Add();
                    Ab8.Range.Text = $"Дата окончания исполнения обозначенной услуги: {TxtEndDate.Text}.";
                    Ab8.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab8.Range.Font.Name = "Times New Roman";
                Ab8.Range.Font.Size = 14;
                Ab8.Range.InsertParagraphAfter();

                    Word.Paragraph Ab9 = doc.Paragraphs.Add();
                    Ab9.Range.Text = $"2. Сумма договора и порядок расчетов";
                    Ab9.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Ab9.Range.Font.Name = "Times New Roman";
                Ab9.Range.Font.Size = 14;
                Ab9.Range.InsertParagraphAfter();

                    Word.Paragraph Ab10 = doc.Paragraphs.Add();
                    Ab10.Range.Text = $"2.1. Сумма настоящего Договора составляет {TxtCost.Text}, включая НДС 20% от стоимости работ.";
                    Ab10.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab10.Range.Font.Name = "Times New Roman";
                Ab10.Range.Font.Size = 14;
                Ab10.Range.InsertParagraphAfter();

                    Word.Paragraph Ab11 = doc.Paragraphs.Add();
                    Ab11.Range.Text = $"2.2. Оплата по настоящему Договору производится в течение 15 (пятнадцати) рабочих дней с момента подписания Договора.";
                    Ab11.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab11.Range.Font.Name = "Times New Roman";
                Ab11.Range.Font.Size = 14;
                Ab11.Range.InsertParagraphAfter();

                    Word.Paragraph Ab12 = doc.Paragraphs.Add();
                    Ab12.Range.Text = $"3. Права и обязанности сторон";
                    Ab12.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Ab12.Range.Font.Name = "Times New Roman";
                Ab12.Range.Font.Size = 14;
                Ab12.Range.InsertParagraphAfter();

                    Word.Paragraph Ab13 = doc.Paragraphs.Add();
                    Ab13.Range.Text = $"3.1. Исполнитель обязан:";
                    Ab13.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab13.Range.Font.Name = "Times New Roman";
                Ab13.Range.Font.Size = 14;
                Ab13.Range.InsertParagraphAfter(); Word.Paragraph Ab14 = doc.Paragraphs.Add();
                    Ab14.Range.Text = $"3.1.1. Оказать услуги надлежащего качества.";
                    Ab14.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab14.Range.Font.Name = "Times New Roman";
                Ab14.Range.Font.Size = 14;
                Ab14.Range.InsertParagraphAfter(); Word.Paragraph Ab15 = doc.Paragraphs.Add();
                    Ab15.Range.Text = $"3.1.2. Оказать услуги в полном объеме в срок, указанный в п. 8.1 настоящего Договора.";
                    Ab15.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab15.Range.Font.Name = "Times New Roman";
                Ab15.Range.Font.Size = 14;
                Ab15.Range.InsertParagraphAfter(); Word.Paragraph Ab16 = doc.Paragraphs.Add();
                    Ab16.Range.Text = $"3.1.3. Безвозмездно исправить по требованию Заказчика все выявленные недостатки, если в процессе оказания услуг Исполнитель допустил отступление от условий Договора, ухудшившее качество работы, в течение 1 (одного) календарного дня дней.";
                    Ab16.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab16.Range.Font.Name = "Times New Roman";
                Ab16.Range.Font.Size = 14;
                Ab16.Range.InsertParagraphAfter(); Word.Paragraph Ab17 = doc.Paragraphs.Add();
                    Ab17.Range.Text = $"3.1.4. Выполнить работу лично или с привлечением третьих лиц.";
                    Ab17.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab17.Range.Font.Name = "Times New Roman";
                Ab17.Range.Font.Size = 14;
                Ab17.Range.InsertParagraphAfter(); Word.Paragraph Ab18 = doc.Paragraphs.Add();
                    Ab18.Range.Text = $"3.2. Заказчик обязан:";
                    Ab18.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab18.Range.Font.Name = "Times New Roman";
                Ab18.Range.Font.Size = 14;
                Ab18.Range.InsertParagraphAfter(); Word.Paragraph Ab19 = doc.Paragraphs.Add();
                    Ab19.Range.Text = $"3.2.1. Оплатить услуги по цене, указанной в п. 2.1. настоящего Договора.";
                    Ab19.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab19.Range.Font.Name = "Times New Roman";
                Ab19.Range.Font.Size = 14;
                Ab19.Range.InsertParagraphAfter(); Word.Paragraph Ab20 = doc.Paragraphs.Add();
                    Ab20.Range.Text = $"3.3. Заказчик имеет право:";
                    Ab20.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab20.Range.Font.Name = "Times New Roman";
                Ab20.Range.Font.Size = 14;
                Ab20.Range.InsertParagraphAfter(); Word.Paragraph Ab21 = doc.Paragraphs.Add();
                    Ab21.Range.Text = $"3.3.1. Во всякое время проверять ход и качество работы, выполняемой Исполнителем, не вмешиваясь в его деятельность.";
                    Ab21.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab21.Range.Font.Name = "Times New Roman";
                Ab21.Range.Font.Size = 14;
                Ab21.Range.InsertParagraphAfter(); Word.Paragraph Ab22 = doc.Paragraphs.Add();
                    Ab22.Range.Text = $"3.3.2. Отказаться от исполнения Договора в любое время до подписания акта оказанных услуг, уплатив Исполнителю часть установленной цены пропорционально части оказанных услуг, выполненной до получения извещения об отказе Заказчика от исполнения договора.";
                    Ab22.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab22.Range.Font.Name = "Times New Roman";
                Ab22.Range.Font.Size = 14;
                Ab22.Range.InsertParagraphAfter();

                    Word.Paragraph Ab23 = doc.Paragraphs.Add();
                    Ab23.Range.Text = $"4. Ответственность сторон";
                   Ab23.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                Ab23.Range.Font.Name = "Times New Roman";
                Ab23.Range.Font.Size = 14;
                Ab23.Range.InsertParagraphAfter();

                    Word.Paragraph Ab24 = doc.Paragraphs.Add();
                    Ab24.Range.Text = $"4.1. За нарушение срока оказания услуг, указанного в п.8.1 настоящего Договора, Исполнитель, при наличии письменной претензии, уплачивает Заказчику пеню в размере 0,1 % (ноль целых одна десятая процента) от суммы Договора за каждый день просрочки.";
                    Ab24.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab24.Range.Font.Name = "Times New Roman";
                Ab24.Range.Font.Size = 14;
                Ab24.Range.InsertParagraphAfter();
                    Word.Paragraph Ab25 = doc.Paragraphs.Add();
                    Ab25.Range.Text = $"4.2. При несоблюдении предусмотренных настоящим Договором сроков расчета за оказанные услуги Заказчик, при наличии письменной претензии, уплачивает Исполнителю пеню в размере 0,1 % (ноль целых одна десятая процента) не перечисленной в срок суммы за каждый день просрочки.";
                    Ab25.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab25.Range.Font.Name = "Times New Roman";
                Ab25.Range.Font.Size = 14;
                Ab25.Range.InsertParagraphAfter();
                    Word.Paragraph Ab26 = doc.Paragraphs.Add();
                    Ab26.Range.Text = $"4.3. Уплата неустойки не освобождает Исполнителя от выполнения лежащих на нем обязательств или устранения нарушений.";
                    Ab26.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                Ab26.Range.Font.Name = "Times New Roman";
                Ab26.Range.Font.Size = 14;
            }
            else
            {
                MessageBox.Show("Введите необходимые данные!");
            }
        }
        private void ReplaceWordDatas(string data, string text, Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: data, ReplaceWith: text);
        }

        private void cmbTask3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTask3.SelectedItem != null)
            {
                StCost.Visibility = Visibility.Visible;
            }
            else
            {
                StCost.Visibility = Visibility.Collapsed;
            }
        }

        private void btnUpdate1_Click(object sender, RoutedEventArgs e)
        {
            DpStartDate.Text = "";
            TxtEndDate.Text = "";
            btnCost.Visibility = Visibility.Collapsed;
            StCost.Visibility = Visibility.Collapsed;
            TxtTask1.Visibility = Visibility.Collapsed;
            cmbTask1.Visibility = Visibility.Collapsed;
            TxtTask2.Visibility = Visibility.Collapsed;
            cmbTask2.Visibility = Visibility.Collapsed;
            TxtTask3.Visibility = Visibility.Collapsed;
            cmbTask3.Visibility = Visibility.Collapsed;
            cmbService.SelectedIndex = -1;
            cmbTask1.SelectedIndex = -1;
            cmbTask2.SelectedIndex = -1;
            cmbTask3.SelectedIndex = -1;
            cmbSortPrice.SelectedIndex = -1;
            TxtCost.Text = "";
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
            LvOrders.VerticalAlignment = VerticalAlignment.Top;
            LvOrders.Height = 894;
        }

        private void BtnVisibilityCalculation_Click(object sender, RoutedEventArgs e)
        {
            if (stRaschet.Visibility == Visibility.Collapsed)
            {
                BtnSformRaschet.Visibility = Visibility.Visible;
                stRaschet.Visibility = Visibility.Visible;
                LvOrders.Visibility = Visibility.Collapsed;
                StOrders.Visibility = Visibility.Collapsed;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                TxtZagolovok.Text = "Калькулятор";
            }
                
        }

        private void BtnVisibilityOrders_Click(object sender, RoutedEventArgs e)
        {
            if (LvOrders.Visibility == Visibility.Collapsed)
            {
                BtnSformRaschet.Visibility = Visibility.Collapsed;
                stRaschet.Visibility = Visibility.Collapsed;
                LvOrders.Visibility = Visibility.Visible;
                StOrders.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Collapsed;
                TxtTasks.Visibility = Visibility.Collapsed;
                LvOrders.VerticalAlignment = VerticalAlignment.Top;
                LvOrders.Height = 894;
                TxtZagolovok.Text = "Заказы";
            }
        }
        private void SortPoisk()
        {
            ListOrders = db.Projects.ToList();
            if (cmbSortPrice.SelectedIndex == 0)
            {
                ListOrders = ListOrders.OrderByDescending(rec => rec.Cost).ToList();
            }
            if (cmbSortPrice.SelectedIndex == 1)
            {
                ListOrders = ListOrders.OrderBy(rec => rec.Cost).ToList();
            }
            DateTime dateM;
            if (DpAppointmentDate.SelectedDate.HasValue)
            {
                dateM = DpAppointmentDate.SelectedDate.Value;
                ListOrders = ListOrders.Where(rec => rec.OrderAcceptanceDate == dateM).ToList();
            }
            else
            {
                var datas = db.Projects.ToList();
                LvOrders.ItemsSource = datas;
            }
            LvOrders.VerticalAlignment = VerticalAlignment.Top;
            LvOrders.Height = 864;
            LvTasks.Visibility = Visibility.Collapsed;
            TxtTasks.Visibility = Visibility.Collapsed;
            LvOrders.ItemsSource = ListOrders;
        }

        private void cmbSortPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortPoisk();

        }

        private void DpAppointmentDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SortPoisk();
        }

        private void BtnDeleteEmployees_Click(object sender, RoutedEventArgs e)
        {
            var order = (Projects)LvOrders.SelectedItem;
            if (order == null)
            {
                MessageBox.Show("Заказ не выбран!");
            }
            else
            {
                MessageBoxResult f = MessageBox.Show("Вы действительно хотите удалить заказ?", "Удаление", MessageBoxButton.YesNo);
                if (f == MessageBoxResult.Yes)
                {
                    db.Projects.Remove(order);
                    if (assignedTasks.IdProject == order.IdProject)
                    {
                        db.AssignedTasks.Remove(assignedTasks);
                    }
                    db.SaveChanges();
                    LvOrders.ItemsSource = db.Projects.ToList();
                    LvOrders.VerticalAlignment = VerticalAlignment.Top;
                    LvOrders.Height = 894;
                    LvTasks.Visibility = Visibility.Collapsed;
                    TxtTasks.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LvOrders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var merActiv = LvOrders.SelectedItem as Projects;
            if (merActiv == null)
            {
                MessageBox.Show("Заказ не выбран!");
            }
            else
            {
                LvOrders.Height = 460;
                LvOrders.VerticalAlignment = VerticalAlignment.Top;

                var istActiv = db.AssignedTasks.Where(rec => rec.IdProject == merActiv.IdProject).ToList();
                LvTasks.ItemsSource = istActiv;

                TxtTasks.Visibility = Visibility.Visible;
                LvTasks.Visibility = Visibility.Visible;
            }
        }

        private void cmbKolVoTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbKolVoTasks.SelectedIndex == 0)
            {
                StTask1.Visibility = Visibility.Visible;
                StTask2.Visibility = Visibility.Collapsed;
                cmbTask2.SelectedIndex = -1;
                StTask3.Visibility = Visibility.Collapsed;
                cmbTask3.SelectedIndex = -1;
                StCost.Visibility = Visibility.Visible;
            }
            else if (cmbKolVoTasks.SelectedIndex == 1)
            {
                StTask1.Visibility = Visibility.Visible;
                StTask2.Visibility = Visibility.Visible;
                StTask3.Visibility = Visibility.Collapsed;
                cmbTask3.SelectedIndex = -1;
                StCost.Visibility = Visibility.Visible;
            }
            else if (cmbKolVoTasks.SelectedIndex == 2)
            {
                StTask1.Visibility = Visibility.Visible;
                StTask2.Visibility = Visibility.Visible;
                StTask3.Visibility = Visibility.Visible;
                StCost.Visibility = Visibility.Visible;
            }
        }

        private void cmbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbClient.SelectedIndex == 0)
            {
                StNewClient.Visibility = Visibility.Visible;
            }
            else
            {
                StNewClient.Visibility = Visibility.Collapsed;
            }
        }


        private void TxtSurname_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtSurname.Clear();
        }

        private void TxtName_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtName.Clear();
        }

        private void TxtPatronymic_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtPatronymic.Clear();
        }

        private void TxtPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtPhone.Clear();
        }

        private void TxtEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            TxtEmail.Clear();
        }

        private bool CheckPhone()
        {
            string phone = TxtPhone.Text;
            Regex phoneRegex = new Regex(@"^8[0-9]{10}");
            if (phoneRegex.IsMatch(phone)) return true;
            else
            {
                MessageBox.Show("Неверный формат телефона");
                return false;
            }
        }
        private bool CheckDatas()
        {
            string surname = TxtSurname.Text;
            string name = TxtName.Text;
            string patronymic = TxtPatronymic.Text;
            string email = TxtEmail.Text;
            if (surname != null && name != null && patronymic != null && email != null) return true;
            else
            {
                MessageBox.Show("Введите все необходимые данные");
                return false;
            }
        }
        private void ClearUserInput()
        {
            TxtSurname.Text = "";
            TxtName.Text = "";
            TxtPatronymic.Text = "";
            TxtEmail.Text = "";
            TxtPhone.Text = "";
        }

        private void BtnRegistrClient_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPhone() == true && CheckDatas() == true)
            {
                user.IdUser = db.Users.OrderByDescending(rec => rec.IdUser).FirstOrDefault().IdUser + 1;
                user.Surname = TxtSurname.Text;
                user.Name = TxtName.Text;
                user.Patronymic = TxtPatronymic.Text;
                user.IdRole = 4;                        
                user.Phone = TxtPhone.Text;
                user.Email = TxtEmail.Text;
                db.Users.Add(user);
                db.SaveChanges();
                MessageBox.Show("Вы успешно добавили нового клиента!");
                ClearUserInput();
                StNewClient.Visibility = Visibility.Collapsed;
                cmbClient.Items.Clear();
                cmbClient.Items.Add("Новый клиент");
                int[] clients = db.Users.Where(rec => rec.IdRole == 4).Select(p => p.IdUser).Distinct().ToArray();
                foreach (int client in clients)
                {
                    cmbClient.Items.Add(client);
                }
                cmbClient.SelectedIndex = -1;

            }
            else
            {
                MessageBox.Show("Не удалось добавить!");
            }
        }
    }
}
