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
using System.Reflection.Emit;

//����ȭ���� 9��30�п� �����׽�Ʈ 4�� 13�� 8�� ~ 9�� ����

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        private int BrowserNumber;
        private int TotalBrowservolume;
        private int tryingRegisterCount;
        private bool bShouldProgramPause;
        private bool bShouldExitProgram;
        private bool bResisterSuccess;

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
            bResisterSuccess = false;

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

            while (true)
            {
                while (dayTag == null) //���� �������� �����ؾ��ϴ� ���⣭�����ϡ� �� �������� �ʴ°�� ���� ���� �Ѿ
                {
                    try
                    {
                        dayTag = driver.FindElement(By.XPath("//a[contains(@onclick, '" + purposeYearMonthDay + "')]"));
                    }
                    catch (NoSuchElementException)
                    {
                        jsExecutor.ExecuteScript("fnGetMonth('+');");
                    }
                }

                //MaxValue �� �ߴ��� �������� 1������ ����
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromMinutes(60));

                while (true)
                {
                    try // ��� ��ư�� �������� �ִ� 1�� ���� ��ٸ� 
                    {
                        wait.Until(wd => !bShouldProgramPause); //bShouldProgramPause �� false �϶� ����������
                        break;
                    }
                    catch (WebDriverTimeoutException) { continue; } // 1�� ������ ��� 1�о� �����ϸ鼭 ��ٸ�
                }

                if (bShouldExitProgram == true || bResisterSuccess == true)
                { 
                    ExitBooking(driver); // ������ ���� ��ư ������ ������ â �̰ŻӸ��ƴ϶� ��ΰ� �����
                    return;
                }

                try
                {
                    //�Լ��� HTML�� �����ִ� �̸� �״�� �ϸ� ����� ���� ���ϰ� �̷��� ��Ȯ�ϰ� XPath �� �ش� �±׿� �����ؼ� onclick �� �Լ��� ȣ���ؾߵ�����
                    // �̺κп� ����ؼ� ���ΰ�ħ �ϸ鼭
                    // fnChoiceDate(this, '2024-03-24', 'IMPOSS'); -> fnChoiceDate(this, '2024-03-24', 'POSS'); �ιٲ𶧱��� ����ؾ���
                    jsExecutor.ExecuteScript("arguments[0].onclick()", dayTag);

                    /*�ּ� : hourMinuteTagsCollection
                    �ش� HTML �±�
                    < a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
                    'fnChoiceCourseTime(this,' �� onclick �� ���ԵǾ��ִ� a �±׸� ã�´� 
                    �װ͵��� Collection�� �߰��Ѵ�.
                    */
                    if (bResisterSuccess == false)
                        setStatusLabe((tryingRegisterCount++).ToString() + " ��° �õ���", browserNumber);

                    // onclick �Ӽ����� ���� ���� �� �����ϴ� ���ٽ� ��� �ð� ����
                    IReadOnlyCollection<IWebElement> hourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));
                    sortedList = hourMinuteTagsCollection.Select(tag => new
                    {
                        Element = tag,
                        SortKey = int.TryParse(tag.GetAttribute("onclick").Substring(Math.Max(0, tag.GetAttribute("onclick").Length - 7), 4), out int number) ? number : int.MaxValue
                    })
                        .OrderBy(item => item.SortKey)
                        .Select(item => item.Element)
                        .ToList();
                }
                catch (UnhandledAlertException)
                {
                    driver.Navigate().Refresh(); // ���� Ŭ���� alert �� �߸� ������ ���ΰ�ħ

                    //���� ������ �ϴ� ���±��� ��ٸ��� : �ִ��ٸ��� �ð� 1��
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    dayTag = null; // null �� �����س��� ���� while ������ �ٽ� ã�� �������� ���ΰ�ħ�ϸ� �ٽ� ã�ƾߵ�
                    continue;
                }
                catch (NoSuchElementException)
                {
                    driver.Navigate().Refresh();

                    //���� ������ �ϴ� ���±��� ��ٸ��� : �ִ��ٸ��� �ð� 1��
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    dayTag = null; // ���ΰ�ħ�ϸ� ���� ��� ã�ƾ���
                    continue;
                }

                // �������� ���� �� �̿� ���� üũ �̸��صα�
                jsExecutor.ExecuteScript("document.getElementById('golfAgreeY').checked = true;");

                // �޽��� ���� ���� �ź�
                jsExecutor.ExecuteScript("document.getElementById('send_sms_yn').checked = false;");

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

                            if (bResisterSuccess == false)
                            {
                                //jsExecutor.ExecuteScript("fnReservation()"); // �׽�Ʈ�Ҷ��� ���� ��ϵǴ°� �������� �ּ��� �Ұ�
                                setStatusLabe( browserNumber.ToString() + "�� ������ ���̽��� ! " + 
                                    purposeYearMonthDay + " " + (hourMinuteToClick / 100).ToString() + ":" + (hourMinuteToClick % 100).ToString() + " ����Ϸ�");
                                // ���� ���� ��Ʈ ��Ÿ�Ͽ� 'Bold'�� �߰��Ͽ� �� ��Ʈ ��Ÿ���� �����մϴ�.
                                statusLabe.ForeColor = Color.Blue;
                                FontStyle newStyle = statusLabe.Font.Style | FontStyle.Bold;

                                // ���ο� ��Ÿ���� �����Ͽ� ���� ��Ʈ�� ������Ʈ�մϴ�.
                                statusLabe.Font = new Font(statusLabe.Font, newStyle);

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
            bShouldExitProgram = false; // �ٽ� ���� ������ ���·� ���� true �� �����
            startButton.Enabled = false; // ���� �߿��� ���� ��ư�� ��Ȱ��ȭ
            stopButton.Enabled = true; // �Ͻ� ���� ��ư Ȱ��ȭ
            exitBrowsersButton.Enabled = true; // ���� ��ư Ȱ��ȭ

            List<Task> bookingTasks = new List<Task>();

            for (int i = 0; i < selectBrowserVolume.Value && bShouldExitProgram == false; i++)
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


            startButton.Enabled = true; // ���� ������ ���� ��ư�� Ȱ��ȭ
            stopButton.Enabled = false; // �Ͻ� ���� ��ư ��Ȱ��ȭ
            exitBrowsersButton.Enabled = false; // ���� ��ư ��Ȱ��ȭ
            BrowserNumber = 1; // �������� ��ȣ�� ��Ÿ����  BrowserNumber �ʱ�ȭ
            tryingRegisterCount = 0; // �õ� Ƚ�� ī��Ʈ �ʱ�ȭ
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
        }
    
        private void setStatusLabe(string status, int browserNumber)
        {
            if (bResisterSuccess == false)
                statusLabe.Text = browserNumber.ToString() + "�� ������ ���� : " + status;
        }

        private void setStatusLabe(string status)
        {
            if (bResisterSuccess == false)
                statusLabe.Text = "���� : " + status;
        }
    }
}
    