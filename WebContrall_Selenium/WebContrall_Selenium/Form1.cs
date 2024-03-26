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

//담주화요일 9시30분에 최종테스트 4월 13일 8시 ~ 9시 사이

namespace WebContrall_Selenium
{
    public partial class Form1 : Form
    {
        // SetThreadExecutionState Win32 API 함수를 선언합니다.
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

            startButton.Enabled = true; // 실행 버튼을 활성화
            stopButton.Enabled = false; // 일시 정지 버튼 비활성화
            exitBrowsersButton.Enabled = false; // 종료 버튼 비활성화

            setStatusLabe("실행 전");

            // WebDriverManager를 사용하여 ChromeDriver를 자동으로 설정합니다.
            new DriverManager().SetUpDriver(new ChromeConfig());
        }

        private void LoginAndGoToReservationAndReady(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        {
            //로그인 페이지로 이동
            driver.Navigate().GoToUrl("https://www.elysian.co.kr/member/login.asp");

            // 자바스크립트 요소를 통해서 요소가 생성되기 전에도 클릭가능
            // 아이디 입력 필드 찾기 ,아이디 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // 비밀번호 입력 필드 찾기 , 비밀번호 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");

            //예약 페이지로 이동
            driver.Navigate().GoToUrl("https://www.elysian.co.kr/reservation/golf.asp");

            while (true)
            {
                try // 10초를 기다려도 페이지가 동작 안할경우 다시 페이지 불러옴
                {
                    //웹이 반응을 하는 상태까지 기다리기 : 최대기다리는 초 10초
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    break;
                }
                catch (WebDriverTimeoutException)
                {
                    //예약 페이지로 이동
                    driver.Navigate().GoToUrl("https://www.elysian.co.kr/reservation/golf.asp");
                }
            }
        }

        private void SelectTimeAndExecuteBooking(IWebDriver driver, IJavaScriptExecutor jsExecutor, int browserNumber)
        {
            //부킹 목적 날자에서 날자 (예시로 2024-03-24 형태) 만 추출함
            string purposeYearMonthDay = dateTimePicker.Value.ToString().Substring(0, 10);

            /*주석 : dayTag 
            //해당 HTML 태그
            <a href="javascript:void(0);" title="예약가능" onclick="fnChoiceDate(this, '2024-03-24', 'POSS');" class="golfResvDate poss">24</a>
            POSS 는 possible 이라는 뜻이었음 날자가 지나면 'END' 예약이 다 된 날짜는 'IMPOSS'
            XPath 의 contains 문법을 사용하여서 목적 날자와 (ex '2024-03-24' 식의형태) 일치하는 태그 찾기
            */
            IWebElement dayTag = null;
            List<IWebElement> sortedList = null; // 선택된 날자의 모든 예약시간 태그를 담을 List
            //MaxValue 로 했더니 에러나서 1분으로 했음
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            bool haveToRefresh = false;
            while (true)
            {
                while (dayTag == null) //현재 페이지에 예약해야하는 ‘년－월－일’ 이 존재하지 않는경우 다음 월로 넘어감
                {
                    try
                    {
                        //dayTag 의 onClick 이 fnChoiceDate(this, '2024-03-24', 'IMPOSS' -> 'POSS'); 로바뀔때까지 코드 반복실행
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

                    setStatusLabe((++tryingRegisterCount).ToString() + " 번째 시도중", browserNumber);

                    /*주석 : hourMinuteTagsCollection
                    XPath 로 해당 태그에 접근해서 onclick 의 함수를 호출해야동작함
                    < a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
                    'fnChoiceCourseTime(this,' 가 onclick 에 포함되어있는 a 태그를 찾는다 
                    그것들을 Collection에 추가한다.
                    */
                    IReadOnlyCollection<IWebElement> hourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));
                    
                    // onclick 속성에서 숫자 추출 및 정렬하는 람다식 사용 시간 정렬
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
                    // '일시 정지' 버튼이 눌려지면 '다시 동작' 을 누르기 전까지 대기하기
                    if (bShouldProgramPause == true)
                    {
                        wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
                        while (true)
                        {
                            try
                            {
                                // 1분 넘으면 계속 1분씩 연장하면서 기다림 여기서 .MaxValue 로 했더니 오류나서 이렇게함
                                wait.Until(wd => (bShouldProgramPause == false));
                                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                                break;
                            }
                            catch (WebDriverTimeoutException) { continue; }
                        }
                    }

                    // 모든 브라우저 종료
                    if (bShouldExitProgram == true || bResisterSuccess == true)
                    {
                        ExitBooking(driver);
                        return;
                    }

                    driver.Navigate().Refresh(); //페이지 새로고침
                    wait.Until(wd => jsExecutor.ExecuteScript("return document.readyState").ToString() == "complete");
                    dayTag = null; // 페이지를 새로고침하면 요소를 다시 찾아야 함
                    haveToRefresh = false;
                    continue;
                }

                foreach (IWebElement hourMinuteTag in sortedList)
                {
                    //onClickVaue 는 onclick 시 호출되는자바스크립트 함수를 문자열로 나타낸것이다.
                    string onClickValue = hourMinuteTag.GetAttribute("onclick");

                    //숫자부분은 항상 끝에서 7번째 에서부터 4글자이므로 이를 이용한다.
                    int startPoint = onClickValue.Length - 7;
                    int purposeHourMinute = dateTimePicker.Value.Hour * 100 + dateTimePicker.Value.Minute;
                    int hourMinuteToClick = int.Parse(onClickValue.Substring(startPoint, 4));

                    if (hourMinuteToClick >= purposeHourMinute)
                    {
                        try
                        {
                            jsExecutor.ExecuteScript("arguments[0].onclick()", hourMinuteTag);

                            // 개인정보 수집 및 이용 동의 체크 미리해두기 시간대 클릭 후가 가장 시간이 여유롭기에, 시간대 클릭부터 함
                            jsExecutor.ExecuteScript("document.getElementById('golfAgreeY').checked = true;");

                            // 메시지 수신 동의 거부
                            jsExecutor.ExecuteScript("document.getElementById('send_sms_yn').checked = false;");

                            if (bResisterSuccess == false || bShouldExitProgram == false)
                            {
                                jsExecutor.ExecuteScript("fnReservation()"); // 테스트할때는 실제 등록되는걸 막기위해 주석을 할것
                                setStatusLabe(browserNumber.ToString() + "번 브라우저 나이스샷 ! " +
                                           purposeYearMonthDay + " " + (hourMinuteToClick / 100).ToString() + ":" + (hourMinuteToClick % 100).ToString() + " 예약완료");
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

                setStatusLabe("현재 선택하신 날자 시간대 이후에 등록 가능한 시간이 없어서 종료됩니다");
                bResisterImpossible = true;
                ExitBooking(driver);
                return;
            }
        }

        private void ExitBooking(IWebDriver driver)
        {
            driver.Quit();
            TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (--TotalBrowservolume).ToString();
        }

        private void combineAllBookingProcesses(IWebDriver driver, IJavaScriptExecutor jsExecutor, int browserNumber)
        {
            try
            {
                setStatusLabe("로그인 및 예약 페이지로 이동중", browserNumber);
                LoginAndGoToReservationAndReady(driver, jsExecutor);

                setStatusLabe("날자 및 시간 선택후 등록중", browserNumber);
                SelectTimeAndExecuteBooking(driver, jsExecutor, browserNumber);
                return;
            }
            catch (JavaScriptException)
            {
                setStatusLabe("예기치 않은 오류 발생하여 종료됨", browserNumber);
                ExitBooking(driver);
                return;
            }
            catch (NoSuchWindowException)
            {
                setStatusLabe("수동으로 창을 닫았거나 예기치 않은 오류 발생하여 종료됨", browserNumber);
                TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (--TotalBrowservolume).ToString();
                return;
            }
            catch (NullReferenceException)
            {
                setStatusLabe("수동으로 창을 닫았거나 예기치 않은 오류 발생하여 종료됨", browserNumber);
                TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (--TotalBrowservolume).ToString();
                return;
            }
            catch (ObjectDisposedException)
            {
                setStatusLabe("수동으로 창을 닫았거나 예기치 않은 오류 발생하여 종료됨", browserNumber);
                TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (--TotalBrowservolume).ToString();
                return;
            }
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            // 시스템이 자동으로 절전 모드로 들어가지 않도록 설정합니다.
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);

            startButton.Enabled = false; // 실행 중에는 실행 버튼을 비활성화
            stopButton.Enabled = true; // 일시 정지 버튼 활성화
            exitBrowsersButton.Enabled = true; // 종료 버튼 활성화

            bShouldExitProgram = false; // 다시 실행 가능한 상태로 설정 true 면 종료됨
            bResisterSuccess = false;  // 성공여부 초기화
            bResisterImpossible = false;

            //상태폰트 기본폰트로 변경
            statusLabe.ForeColor = Color.Black;
            FontStyle newStyle = statusLabe.Font.Style ^ FontStyle.Bold;
            statusLabe.Font = new Font(statusLabe.Font, newStyle);

            List<Task> bookingTasks = new List<Task>();

            for (int i = 0; i < selectBrowserVolume.Value && bShouldExitProgram == false && bResisterSuccess == false && bResisterImpossible == false; i++)
            { 
                // ChromeDriver 서비스를 설정하여 콘솔 창을 숨깁니다.
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true; // 콘솔 창 숨기기

                // Chrome 옵션을 설정합니다.
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless"); // headless 모드 활성화
                options.AddArgument("--disable-gpu"); // 일부 시스템에서 headless 모드를 위해 필요
                                                      // window 사이즈를 설정하므로써 눈에 보이지않는 웹이지만 사이즈별로 올수있는 에러방지
                options.AddArgument("--window-size=1920,1080");

                // 설정된 서비스와 옵션으로 ChromeDriver 인스턴스를 생성합니다.
                // chrome 브라우저와 소통 하게 해주는 ChromeDriver
                IWebDriver driver = new ChromeDriver(service, options);

                /* 주석 : `IJavaScriptExecutor` 인터페이스를 사용하여 Selenium WebDriver에서 JavaScript 코드를 실행할 수 있습니다.
                 이 인터페이스는 테스트 자동화 중 웹 페이지의 자바스크립트 환경에 직접 접근하여 코드를 실행하게 해줍니다.
                 예를 들어, 페이지에 보이지 않는 요소를 클릭하거나, 페이지의 DOM을 직접 조작하는 등의 작업을 할 수 있습니다.
                 이를 통해, 테스트 시나리오에서 요구하는 복잡한 웹 상호작용을 구현할 수 있습니다. 
                */
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

                int browserNumber = BrowserNumber++; // 브라우저에 배정된 번호
                TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (++TotalBrowservolume).ToString();

                //비동기적 호출으로 메인 스레드 외에 다른 스레드로 작업을 함으로써 동시에 여러 브라우저 호출 가능
                Task task = Task.Run(() => combineAllBookingProcesses(driver, jsExecutor, browserNumber));
                await Task.Delay(200); //중간중간에 잠시 멈춰주면서 일시정지또는 정지버튼을 누를 수 있게 해줌
                bookingTasks.Add(task); // 작업 목록에 추가합니다.
            }

            // 모든 예약 작업이 완료되기를 기다립니다.
            await Task.WhenAll(bookingTasks);

            if (bShouldExitProgram == true) setStatusLabe("정상적으로 모든 브라우저가 종료됨");
            else if (bResisterSuccess == true)
            {
                // 라벨의 현재 폰트 스타일에 'Bold'를 추가하여 새 폰트 스타일을 구성합니다.
                statusLabe.ForeColor = Color.Blue;
                newStyle = statusLabe.Font.Style | FontStyle.Bold;

                // 새로운 스타일을 적용하여 라벨의 폰트를 업데이트합니다.
                statusLabe.Font = new Font(statusLabe.Font, newStyle);
            }

            startButton.Enabled = true; // 실행 끝나고 실행 버튼을 활성화
            stopButton.Enabled = false; // 일시 정지 버튼 비활성화
            exitBrowsersButton.Enabled = false; // 종료 버튼 비활성화
            BrowserNumber = 1; // 브라우저의 번호를 나타내는  BrowserNumber 초기화
            tryingRegisterCount = 0; // 시도 횟수 카운트 초기화

            // 프로그램이 종료되기 직전에 전원 설정을 원래대로 복원합니다.
            SetThreadExecutionState(ES_CONTINUOUS);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            bShouldProgramPause = !bShouldProgramPause;

            if (bShouldProgramPause) stopButton.Text = "다시동작";
            else stopButton.Text = "일시정지";
        }

        private void exitBrowsersButton_Click(object sender, EventArgs e)
        {
            bShouldExitProgram = true;
            bShouldProgramPause = false;

            stopButton.Text = "일시정지";
        }
    
        private void setStatusLabe(string status, int browserNumber)
        {
            if (bResisterSuccess == false && bResisterImpossible == false)
                statusLabe.Text = browserNumber.ToString() + "번 브라우저 상태 : " + status;
        }

        private void setStatusLabe(string status)
        {
            if (bResisterSuccess == false && bResisterImpossible == false)
                statusLabe.Text = "상태 : " + status;
        }
    }
}
    