using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V120.FedCm;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using static System.Windows.Forms.LinkLabel;
using System.CodeDom.Compiler;
using System.Windows.Forms;

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void LoginState()
        {
            //chrome �������� �ҵ� �ϰ� ���ִ� ChromeDriver
            IWebDriver driver = new ChromeDriver();
            //�ڹٽ�ũ��Ʈ ��Ҹ� ���ؼ� ��Ұ� �����Ǳ� ������ Ŭ������

            // `IJavaScriptExecutor` �������̽��� ����Ͽ� Selenium WebDriver���� JavaScript �ڵ带 ������ �� �ֽ��ϴ�.
            // �� �������̽��� �׽�Ʈ �ڵ�ȭ �� �� �������� �ڹٽ�ũ��Ʈ ȯ�濡 ���� �����Ͽ� �ڵ带 �����ϰ� ���ݴϴ�.
            // ���� ���, �������� ������ �ʴ� ��Ҹ� Ŭ���ϰų�, �������� DOM�� ���� �����ϴ� ���� �۾��� �� �� �ֽ��ϴ�.
            // �̸� ����, �׽�Ʈ �ó��������� �䱸�ϴ� ������ �� ��ȣ�ۿ��� ������ �� �ֽ��ϴ�.
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            driver.Navigate().GoToUrl("https://www.elysian.co.kr/member/login.asp");

            // ���̵� �Է� �ʵ� ã�� ,���̵� �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // ��й�ȣ �Է� �ʵ� ã�� , ��й�ȣ �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");

            //�α��� ��� �� ���� ���� ����.
            driver.Navigate().GoToUrl("https://www.elysian.co.kr/reservation/golf.asp");


            //���� ������ �ϴ� ���±��� ��ٸ��� : �ִ��ٸ��� �� 10��
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");

            //��ŷ ���� ���ڿ��� ���� (���÷� 2024-03-24 ����) �� ������
            String purposeYearMonthDay = dateTimePicker1.Value.ToString().Substring(0, 10);

            //�±� ����
            //<a href="javascript:void(0);" title="���డ��" onclick="fnChoiceDate(this, '2024-03-24', 'POSS');" class="golfResvDate poss">24</a>
            //POSS �� possible �̶�� ���̾��� ���ڰ� ������ 'END' ������ �� �� ��¥�� 'IMPOSS'
            //XPath �� contains ������ ����Ͽ��� ���� ���ڿ� (ex '2024-03-24' ��������) ��ġ�ϴ� �±� ã��
            IWebElement YearMonthDayTag = driver.FindElement(By.XPath("//a[contains(@onclick, '" + purposeYearMonthDay + "')]"));

            //�Լ��� HTML�� �����ִ� �̸� �״�� �ϸ� ����� ���� ���ϰ� �̷��� ��Ȯ�ϰ�
            //XPath �� �ش� �±׿� �����ؼ� onclick �� �Լ��� ȣ���ؾߵ�����
            jsExecutor.ExecuteScript("arguments[0].onclick()", YearMonthDayTag);

            //�±� ����
            //< a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
            //'fnChoiceCourseTime(this,' �� onclick �� ���ԵǾ��ִ� a �±׸� ã�´� 
            IReadOnlyCollection<IWebElement> HourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));

            foreach (var HourMinuteTag in HourMinuteTagsCollection)
            {
                string onClickValue = HourMinuteTag.GetAttribute("onclick");
                
                //onClickVaue �� onclick �� ȣ��Ǵ��ڹٽ�ũ��Ʈ �Լ��� ���ڿ��� ��Ÿ�����̴�.
                //���ںκ��� �׻� ������ 7��° �������� 4�����̹Ƿ� �̸� �̿��Ѵ�.
                int startPoint = onClickValue.Length - 7;
                int purposeHourMinute = dateTimePicker1.Value.Hour * 100 + dateTimePicker1.Value.Minute;
                if (int.Parse(onClickValue.Substring(startPoint, 4)) >= purposeHourMinute)
                {
                    jsExecutor.ExecuteScript("arguments[0].onclick()", HourMinuteTag);
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginState();
        }

        private void dateLable_Click(object sender, EventArgs e)
        {

        }
    }
}
