using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V120.FedCm;

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        //chrome �������� �ҵ� �ϰ� ���ִ� ChromeDriver
        private IWebDriver driver;
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
            driver = new ChromeDriver();
            //�ڹٽ�ũ��Ʈ ��Ҹ� ���ؼ� ��Ұ� �����Ǳ� ������ Ŭ������
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            driver.Navigate().GoToUrl("https://m.elysian.co.kr/member/login.asp");

            // ���̵� �Է� �ʵ� ã�� ,���̵� �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // ��й�ȣ �Է� �ʵ� ã�� , ��й�ȣ �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");

            //// ���̵� ���� üũ�ڽ� ã��
            //IWebElement SaveIdCheckbox = driver.FindElement(By.Id("id_save"));
            //SaveIdCheckbox.Click();
            //
            //// �ڵ��α��� üũ�ڽ� ã��
            //IWebElement LoggedInAutoCheckbox = driver.FindElement(By.Id("login_auto"));
            //LoggedInAutoCheckbox.Click();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginState();
            driver.Navigate().GoToUrl("https://m.elysian.co.kr/reservation/golf_choice.asp");
        }
    }
}
