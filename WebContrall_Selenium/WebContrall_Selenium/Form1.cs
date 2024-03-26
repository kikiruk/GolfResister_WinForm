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
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using AngleSharp.Dom;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

//����ȭ���� 9��30�п� �����׽�Ʈ 4�� 13�� 8�� ~ 9�� ����

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        // SetThreadExecutionState Win32 API �Լ��� �����մϴ�.
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);

        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;

        private int BrowserNumber;
        private int TotalBrowservolume;
        private int tryingRegisterCount;
        private bool bShouldProgramPause;
        private bool bShouldExitProgram;
        private bool bResisterSuccess;
        private bool bResisterImpossible;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BrowserNumber = 1;
            TotalBrowservolume = 0;
            tryingRegisterCount = 0;
            bShouldProgramPause = false;
            bShouldExitProgram = false;
            bResisterSuccess = true;
            bResisterImpossible = false;

            startButton.Enabled = true; // ���� ��ư�� Ȱ��ȭ
            stopButton.Enabled = false; // �Ͻ� ���� ��ư ��Ȱ��ȭ
            exitBrowsersButton.Enabled = false; // ���� ��ư ��Ȱ��ȭ

            setStatusLabe("���� ��");

            // WebDriverManager�� ����Ͽ� ChromeDriver�� �ڵ����� �����մϴ�.
            new DriverManager().SetUpDriver(new ChromeConfig());
        }

        private void LoginAndGoToReservationAndReady(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        {
            //�α��� �������� �̵�
            driver.Navigate().GoToUrl("https://www.elysian.co.kr/member/login.asp");

            // �ڹٽ�ũ��Ʈ ��Ҹ� ���ؼ� ��Ұ� �����Ǳ� ������ Ŭ������
            // ���̵� �Է� �ʵ� ã�� ,���̵� �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // ��й�ȣ �Է� �ʵ� ã�� , ��й�ȣ �Է�
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");

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
        }

        private void SelectTimeAndExecuteBooking(IWebDriver driver, IJavaScriptExecutor jsExecutor, int browserNumber)
        {
            //��ŷ ���� ���ڿ��� ���� (���÷� 2024-03-24 ����) �� ������
            string purposeYearMonthDay = dateTimePicker.Value.ToString().Substring(0, 10);

            /*�ּ� : dayTag 
            //�ش� HTML �±�
            <a href="javascript:void(0);" title="���డ��" onclick="fnChoiceDate(this, '2024-03-24', 'POSS');" class="golfResvDate poss">24</a>
            POSS �� possible �̶�� ���̾��� ���ڰ� ������ 'END' ������ �� �� ��¥�� 'IMPOSS'
            XPath �� contains ������ ����Ͽ��� ���� ���ڿ� (ex '2024-03-24' ��������) ��ġ�ϴ� �±� ã��
            */
            IWebElement dayTag = null;
            List<IWebElement> sortedList = null; // ���õ� ������ ��� ����ð� �±׸� ���� List
            //MaxValue �� �ߴ��� �������� 1������ ����
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            bool haveToRefresh = false;
            while (true)
            {
                while (dayTag == null) //���� �������� �����ؾ��ϴ� ���⣭�����ϡ� �� �������� �ʴ°�� ���� ���� �Ѿ
                {
                    try
                    {
                        //dayTag �� onClick �� fnChoiceDate(this, '2024-03-24', 'IMPOSS' -> 'POSS'); �ιٲ𶧱��� �ڵ� �ݺ�����
                        dayTag = driver.FindElement(By.XPath("//a[contains(@onclick, '" + purposeYearMonthDay + "')]"));
                    }
                    catch (NoSuchElementException)
                    {
                        jsExecutor.ExecuteScript("fnGetMonth('+');");
                    }
                }

                try
                {
                    jsExecutor.ExecuteScript("arguments[0].onclick()", dayTag);

                    setStatusLabe((++tryingRegisterCount).ToString() + " ��° �õ���", browserNumber);

                    /*�ּ� : hourMinuteTagsCollection
                    XPath �� �ش� �±׿� �����ؼ� onclick �� �Լ��� ȣ���ؾߵ�����
                    < a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
                    'fnChoiceCourseTime(this,' �� onclick �� ���ԵǾ��ִ� a �±׸� ã�´� 
                    �װ͵��� Collection�� �߰��Ѵ�.
                    */
                    IReadOnlyCollection<IWebElement> hourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));
                    
                    // onclick �Ӽ����� ���� ���� �� �����ϴ� ���ٽ� ��� �ð� ����
                    sortedList = hourMinuteTagsCollection.Select(tag => new
                    {
                        Element = tag,
                        SortKey = int.TryParse(tag.GetAttribute("onclick").Substring(Math.Max(0, tag.GetAttribute("onclick").Length - 7), 4), out int number) ? number : int.MaxValue
                    
                    }).OrderBy(item => item.SortKey).Select(item => item.Element).ToList();
                }
                catch (UnhandledAlertException) { haveToRefresh = true; }
                catch (NoSuchElementException) { haveToRefresh = true; }

                if(haveToRefresh)
                {
                    // '�Ͻ� ����' ��ư�� �������� '�ٽ� ����' �� ������ ������ ����ϱ�
                    if (bShouldProgramPause == true)
                    {
                        wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
                        while (true)
                        {
                            try
                            {
                                // 1�� ������ ��� 1�о� �����ϸ鼭 ��ٸ� ���⼭ .MaxValue �� �ߴ��� �������� �̷�����
                                wait.Until(wd => (bShouldProgramPause == false));
                                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                break;
                            }
                            catch (WebDriverTimeoutException) { continue; }
                        }
                    }

                    // ��� ������ ����
                    if (bShouldExitProgram == true || bResisterSuccess == true)
                    {
                        ExitBooking(driver);
                        return;
                    }

                    driver.Navigate().Refresh(); //������ ���ΰ�ħ
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    dayTag = null; // �������� ���ΰ�ħ�ϸ� ��Ҹ� �ٽ� ã�ƾ� ��
                    haveToRefresh = false;
                    continue;
                }

                foreach (IWebElement hourMinuteTag in sortedList)
                {
                    //onClickVaue �� onclick �� ȣ��Ǵ��ڹٽ�ũ��Ʈ �Լ��� ���ڿ��� ��Ÿ�����̴�.
                    string onClickValue = hourMinuteTag.GetAttribute("onclick");

                    //���ںκ��� �׻� ������ 7��° �������� 4�����̹Ƿ� �̸� �̿��Ѵ�.
                    int startPoint = onClickValue.Length - 7;
                    int purposeHourMinute = dateTimePicker.Value.Hour * 100 + dateTimePicker.Value.Minute;
                    int hourMinuteToClick = int.Parse(onClickValue.Substring(startPoint, 4));

                    if (hourMinuteToClick >= purposeHourMinute)
                    {
                        try
                        {
                            jsExecutor.ExecuteScript("arguments[0].onclick()", hourMinuteTag);

                            // �������� ���� �� �̿� ���� üũ �̸��صα� �ð��� Ŭ�� �İ� ���� �ð��� �����ӱ⿡, �ð��� Ŭ������ ��
                            jsExecutor.ExecuteScript("document.getElementById('golfAgreeY').checked = true;");

                            // �޽��� ���� ���� �ź�
                            jsExecutor.ExecuteScript("document.getElementById('send_sms_yn').checked = false;");

                            if (bResisterSuccess == false || bShouldExitProgram == false)
                            {
                                jsExecutor.ExecuteScript("fnReservation()"); // �׽�Ʈ�Ҷ��� ���� ��ϵǴ°� �������� �ּ��� �Ұ�
                                setStatusLabe(browserNumber.ToString() + "�� ������ ���̽��� ! " +
                                           purposeYearMonthDay + " " + (hourMinuteToClick / 100).ToString() + ":" + (hourMinuteToClick % 100).ToString() + " ����Ϸ�");
                                bResisterSuccess = true;
                            }
                        }
                        catch (UnhandledAlertException)
                        {
                            continue;
                        }

                        ExitBooking(driver);
                        return;
                    }
                }

                setStatusLabe("���� �����Ͻ� ���� �ð��� ���Ŀ� ��� ������ �ð��� ��� ����˴ϴ�");
                bResisterImpossible = true;
                ExitBooking(driver);
                return;
            }
        }

        private void ExitBooking(IWebDriver driver)
        {
            driver.Quit();
            TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (--TotalBrowservolume).ToString();
        }

        private void combineAllBookingProcesses(IWebDriver driver, IJavaScriptExecutor jsExecutor, int browserNumber)
        {
            try
            {
                setStatusLabe("�α��� �� ���� �������� �̵���", browserNumber);
                LoginAndGoToReservationAndReady(driver, jsExecutor);

                setStatusLabe("���� �� �ð� ������ �����", browserNumber);
                SelectTimeAndExecuteBooking(driver, jsExecutor, browserNumber);
                return;
            }
            catch (JavaScriptException)
            {
                setStatusLabe("����ġ ���� ���� �߻��Ͽ� �����", browserNumber);
                ExitBooking(driver);
                return;
            }
            catch (NoSuchWindowException)
            {
                setStatusLabe("�������� â�� �ݾҰų� ����ġ ���� ���� �߻��Ͽ� �����", browserNumber);
                TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (--TotalBrowservolume).ToString();
                return;
            }
            catch (NullReferenceException)
            {
                setStatusLabe("�������� â�� �ݾҰų� ����ġ ���� ���� �߻��Ͽ� �����", browserNumber);
                TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (--TotalBrowservolume).ToString();
                return;
            }
            catch (ObjectDisposedException)
            {
                setStatusLabe("�������� â�� �ݾҰų� ����ġ ���� ���� �߻��Ͽ� �����", browserNumber);
                TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (--TotalBrowservolume).ToString();
                return;
            }
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // �ý����� �ڵ����� ���� ���� ���� �ʵ��� �����մϴ�.
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            startButton.Enabled = false; // ���� �߿��� ���� ��ư�� ��Ȱ��ȭ
            stopButton.Enabled = true; // �Ͻ� ���� ��ư Ȱ��ȭ
            exitBrowsersButton.Enabled = true; // ���� ��ư Ȱ��ȭ

            bShouldExitProgram = false; // �ٽ� ���� ������ ���·� ���� true �� �����
            bResisterSuccess = false;  // �������� �ʱ�ȭ
            bResisterImpossible = false;

            //������Ʈ �⺻��Ʈ�� ����
            statusLabe.ForeColor = Color.Black;
            FontStyle newStyle = statusLabe.Font.Style ^ FontStyle.Bold;
            statusLabe.Font = new Font(statusLabe.Font, newStyle);

            List<Task> bookingTasks = new List<Task>();

            for (int i = 0; i < selectBrowserVolume.Value && bShouldExitProgram == false && bResisterSuccess == false && bResisterImpossible == false; i++)
            { 
                // ChromeDriver ���񽺸� �����Ͽ� �ܼ� â�� ����ϴ�.
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true; // �ܼ� â �����

                // Chrome �ɼ��� �����մϴ�.
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless"); // headless ��� Ȱ��ȭ
                options.AddArgument("--disable-gpu"); // �Ϻ� �ý��ۿ��� headless ��带 ���� �ʿ�
                                                      // window ����� �����ϹǷν� ���� �������ʴ� �������� ������� �ü��ִ� ��������
                options.AddArgument("--window-size=1920,1080");

                // ������ ���񽺿� �ɼ����� ChromeDriver �ν��Ͻ��� �����մϴ�.
                // chrome �������� ���� �ϰ� ���ִ� ChromeDriver
                IWebDriver driver = new ChromeDriver(service, options);

                /* �ּ� : `IJavaScriptExecutor` �������̽��� ����Ͽ� Selenium WebDriver���� JavaScript �ڵ带 ������ �� �ֽ��ϴ�.
                 �� �������̽��� �׽�Ʈ �ڵ�ȭ �� �� �������� �ڹٽ�ũ��Ʈ ȯ�濡 ���� �����Ͽ� �ڵ带 �����ϰ� ���ݴϴ�.
                 ���� ���, �������� ������ �ʴ� ��Ҹ� Ŭ���ϰų�, �������� DOM�� ���� �����ϴ� ���� �۾��� �� �� �ֽ��ϴ�.
                 �̸� ����, �׽�Ʈ �ó��������� �䱸�ϴ� ������ �� ��ȣ�ۿ��� ������ �� �ֽ��ϴ�. 
                */
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

                int browserNumber = BrowserNumber++; // �������� ������ ��ȣ
                TotalBrowservolumeLable.Text = "���� �������� ������ �� : " + (++TotalBrowservolume).ToString();

                //�񵿱��� ȣ������ ���� ������ �ܿ� �ٸ� ������� �۾��� �����ν� ���ÿ� ���� ������ ȣ�� ����
                Task task = Task.Run(() => combineAllBookingProcesses(driver, jsExecutor, browserNumber));
                await Task.Delay(200); //�߰��߰��� ��� �����ָ鼭 �Ͻ������Ǵ� ������ư�� ���� �� �ְ� ����
                bookingTasks.Add(task); // �۾� ��Ͽ� �߰��մϴ�.
            }

            // ��� ���� �۾��� �Ϸ�Ǳ⸦ ��ٸ��ϴ�.
            await Task.WhenAll(bookingTasks);

            if (bShouldExitProgram == true) setStatusLabe("���������� ��� �������� �����");
            else if (bResisterSuccess == true)
            {
                // ���� ���� ��Ʈ ��Ÿ�Ͽ� 'Bold'�� �߰��Ͽ� �� ��Ʈ ��Ÿ���� �����մϴ�.
                statusLabe.ForeColor = Color.Blue;
                newStyle = statusLabe.Font.Style | FontStyle.Bold;

                // ���ο� ��Ÿ���� �����Ͽ� ���� ��Ʈ�� ������Ʈ�մϴ�.
                statusLabe.Font = new Font(statusLabe.Font, newStyle);
            }

            startButton.Enabled = true; // ���� ������ ���� ��ư�� Ȱ��ȭ
            stopButton.Enabled = false; // �Ͻ� ���� ��ư ��Ȱ��ȭ
            exitBrowsersButton.Enabled = false; // ���� ��ư ��Ȱ��ȭ
            BrowserNumber = 1; // �������� ��ȣ�� ��Ÿ����  BrowserNumber �ʱ�ȭ
            tryingRegisterCount = 0; // �õ� Ƚ�� ī��Ʈ �ʱ�ȭ

            // ���α׷��� ����Ǳ� ������ ���� ������ ������� �����մϴ�.
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            bShouldProgramPause = !bShouldProgramPause;

            if (bShouldProgramPause) stopButton.Text = "�ٽõ���";
            else stopButton.Text = "�Ͻ�����";
        }

        private void exitBrowsersButton_Click(object sender, EventArgs e)
        {
            bShouldExitProgram = true;
            bShouldProgramPause = false;

            stopButton.Text = "�Ͻ�����";
        }
    
        private void setStatusLabe(string status, int browserNumber)
        {
            if (bResisterSuccess == false && bResisterImpossible == false)
                statusLabe.Text = browserNumber.ToString() + "�� ������ ���� : " + status;
        }

        private void setStatusLabe(string status)
        {
            if (bResisterSuccess == false && bResisterImpossible == false)
                statusLabe.Text = "���� : " + status;
        }
    }
}
    