using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V120.FedCm;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using static System.Windows.Forms.LinkLabel;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using OpenQA.Selenium.DevTools.V120.Debugger;
using static System.Runtime.InteropServices.JavaScript.JSType;

//����ȭ���� 9��30�п� �����׽�Ʈ

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        private int BrowserNumber;
        private int TotalBrowservolume;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            BrowserNumber = 1;
            TotalBrowservolume = 0;
            setStatusLabe("���� ��");
        }

        private void LoginState(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        {
            //�ڹٽ�ũ��Ʈ ��Ҹ� ���ؼ� ��Ұ� �����Ǳ� ������ Ŭ������

            driver.Navigate().GoToUrl("https://www.elysian.co.kr/member/login.asp");

            // ���̵� �Է� �ʵ� ã�� ,���̵� �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // ��й�ȣ �Է� �ʵ� ã�� , ��й�ȣ �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");
        }

        private void GoToReservationAndSelectMonthDateTime(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        { 
            //���� �������� �̵�
            driver.Navigate().GoToUrl("https://www.elysian.co.kr/reservation/golf.asp");
            
            while (true)
            { 
                try // 10�ʸ� ��ٷ��� �������� ���� ���Ұ�� �ٽ� ������ �ҷ���
                {
                    //���� ������ �ϴ� ���±��� ��ٸ��� : �ִ��ٸ��� �� 10��
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    //���� �������� �̵�
                    driver.Navigate().GoToUrl("https://www.elysian.co.kr/reservation/golf.asp");
                }
            }

            //��ŷ ���� ���ڿ��� ���� (���÷� 2024-03-24 ����) �� ������
            string purposeYearMonthDay = dateTimePicker1.Value.ToString().Substring(0, 10);
            /*�ּ� : YearMonthDayTag 
            //�ش� HTML �±�
            <a href="javascript:void(0);" title="���డ��" onclick="fnChoiceDate(this, '2024-03-24', 'POSS');" class="golfResvDate poss">24</a>
            POSS �� possible �̶�� ���̾��� ���ڰ� ������ 'END' ������ �� �� ��¥�� 'IMPOSS'
            XPath �� contains ������ ����Ͽ��� ���� ���ڿ� (ex '2024-03-24' ��������) ��ġ�ϴ� �±� ã��
            */
            IWebElement YearMonthDayTag = null;

            while (true) //���� �������� �����ؾ��ϴ� ���� �������� �ʴ°�� ���� ���� �Ѿ
            {
                try 
                {
                    YearMonthDayTag = driver.FindElement(By.XPath("//a[contains(@onclick, '" + purposeYearMonthDay + "')]"));
                    if (YearMonthDayTag != null) break;
                }
                catch (NoSuchElementException)
                {
                    jsExecutor.ExecuteScript("fnGetMonth('+');");
                }
            }

            //�Լ��� HTML�� �����ִ� �̸� �״�� �ϸ� ����� ���� ���ϰ� �̷��� ��Ȯ�ϰ� XPath �� �ش� �±׿� �����ؼ� onclick �� �Լ��� ȣ���ؾߵ�����
            // �̺κп� ����ؼ� ���ΰ�ħ �ϸ鼭
            // fnChoiceDate(this, '2024-03-24', 'IMPOSS'); -> fnChoiceDate(this, '2024-03-24', 'POSS'); �ιٲ𶧱��� ����ؾ���
            // ���ΰ�ħ �±� : <li><a href="javascript:fnCourseTimeReset();" class="reflash_btn">���ΰ�ħ</a></li> 
            jsExecutor.ExecuteScript("arguments[0].onclick()", YearMonthDayTag); 

            /*�ּ� : HourMinuteTagsCollection
            �ش� HTML �±�
            < a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
            'fnChoiceCourseTime(this,' �� onclick �� ���ԵǾ��ִ� a �±׸� ã�´� 
            �װ͵��� Collection�� �߰��Ѵ�.
            */
            IReadOnlyCollection<IWebElement> HourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));

            foreach (var HourMinuteTag in HourMinuteTagsCollection)
            {
                //onClickVaue �� onclick �� ȣ��Ǵ��ڹٽ�ũ��Ʈ �Լ��� ���ڿ��� ��Ÿ�����̴�.
                string onClickValue = HourMinuteTag.GetAttribute("onclick");

                //���ںκ��� �׻� ������ 7��° �������� 4�����̹Ƿ� �̸� �̿��Ѵ�.
                int startPoint = onClickValue.Length - 7;
                int purposeHourMinute = dateTimePicker1.Value.Hour * 100 + dateTimePicker1.Value.Minute;

                if (int.Parse(onClickValue.Substring(startPoint, 4)) >= purposeHourMinute)
                {
                    jsExecutor.ExecuteScript("arguments[0].onclick()", HourMinuteTag);
                    return;
                }
            }

            CancleBooking(driver);
        }

        private void ExecuteBooking(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        {
            jsExecutor.ExecuteScript("document.getElementById('golfAgreeY').checked = true;");
            //jsExecutor.ExecuteScript("fnReservation()"); // �׽�Ʈ�Ҷ��� ���� ��ϵǴ°� �������� �ּ��� �Ұ�
        }

        private void CancleBooking(IWebDriver driver)
        {
            driver.Quit();
            TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (--TotalBrowservolume).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //chrome �������� ���� �ϰ� ���ִ� ChromeDriver
            IWebDriver driver = new ChromeDriver();
            /* �ּ� : `IJavaScriptExecutor` �������̽��� ����Ͽ� Selenium WebDriver���� JavaScript �ڵ带 ������ �� �ֽ��ϴ�.
             �� �������̽��� �׽�Ʈ �ڵ�ȭ �� �� �������� �ڹٽ�ũ��Ʈ ȯ�濡 ���� �����Ͽ� �ڵ带 �����ϰ� ���ݴϴ�.
             ���� ���, �������� ������ �ʴ� ��Ҹ� Ŭ���ϰų�, �������� DOM�� ���� �����ϴ� ���� �۾��� �� �� �ֽ��ϴ�.
             �̸� ����, �׽�Ʈ �ó��������� �䱸�ϴ� ������ �� ��ȣ�ۿ��� ������ �� �ֽ��ϴ�. 
            */
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            int browserNumber = BrowserNumber++; // �������� ������ ��ȣ
            TotalBrowservolumeLable.Text =  "���� �������� ������ �� : " + (++TotalBrowservolume).ToString();

            setStatusLabe("�α�����", browserNumber);
            LoginState(driver, jsExecutor);

            try
            {
                setStatusLabe("��ŷ �������� �̵� �� ���� �� �ð� ������", browserNumber);
                GoToReservationAndSelectMonthDateTime(driver, jsExecutor);
            }
            catch(UnhandledAlertException)
            {
                CancleBooking(driver);
                setStatusLabe(" ����� : ���ڰ� �ùٸ��� �ԷµǾ����� �ٽ� Ȯ�����ּ���", browserNumber);
                return;
            }

            try
            {
                setStatusLabe("����, �ð� Ȯ���� ���� ��ŷ �����", browserNumber);
                ExecuteBooking(driver, jsExecutor);
            }
            catch (WebDriverException) 
            { 
                setStatusLabe("�ش� ���� ���� ������ �ð� ���� ����", browserNumber);
            }
        }

        private void setStatusLabe(string status, int browserNumber)
        {
            statusLabe.Text = browserNumber.ToString() + "�� ������ ���� : " + status;
        }

        private void setStatusLabe(string status)
        {
            statusLabe.Text = "���� : " + status;
        }
    }
}
