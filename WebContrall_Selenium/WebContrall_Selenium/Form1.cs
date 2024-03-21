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

//담주화요일 9시30분에 최종테스트

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
            setStatusLabe("실행 전");
        }

        private void LoginState(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        {
            //자바스크립트 요소를 통해서 요소가 생성되기 전에도 클릭가능

            driver.Navigate().GoToUrl("https://www.elysian.co.kr/member/login.asp");

            // 아이디 입력 필드 찾기 ,아이디 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_01').value = arguments[0];", "11037301");

            // 비밀번호 입력 필드 찾기 , 비밀번호 입력
            jsExecutor.ExecuteScript("document.getElementById('userinfo01_02').value = arguments[0];", "9999");


            jsExecutor.ExecuteScript("fnLogin(1);");
        }

        private void GoToReservationAndSelectMonthDateTime(IWebDriver driver, IJavaScriptExecutor jsExecutor)
        { 
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

            //부킹 목적 날자에서 날자 (예시로 2024-03-24 형태) 만 추출함
            string purposeYearMonthDay = dateTimePicker1.Value.ToString().Substring(0, 10);
            /*주석 : YearMonthDayTag 
            //해당 HTML 태그
            <a href="javascript:void(0);" title="예약가능" onclick="fnChoiceDate(this, '2024-03-24', 'POSS');" class="golfResvDate poss">24</a>
            POSS 는 possible 이라는 뜻이었음 날자가 지나면 'END' 예약이 다 된 날짜는 'IMPOSS'
            XPath 의 contains 문법을 사용하여서 목적 날자와 (ex '2024-03-24' 식의형태) 일치하는 태그 찾기
            */
            IWebElement YearMonthDayTag = null;

            while (true) //현재 페이지에 예약해야하는 월이 존재하지 않는경우 다음 월로 넘어감
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

            //함수를 HTML에 나와있는 이름 그대로 하면 제대로 동작 안하고 이렇게 정확하게 XPath 로 해당 태그에 접근해서 onclick 의 함수를 호출해야동작함
            // 이부분에 계속해서 새로고침 하면서
            // fnChoiceDate(this, '2024-03-24', 'IMPOSS'); -> fnChoiceDate(this, '2024-03-24', 'POSS'); 로바뀔때까지 대기해야함
            // 새로고침 태그 : <li><a href="javascript:fnCourseTimeReset();" class="reflash_btn">새로고침</a></li> 
            jsExecutor.ExecuteScript("arguments[0].onclick()", YearMonthDayTag); 

            /*주석 : HourMinuteTagsCollection
            해당 HTML 태그
            < a href = "javascript:void(0);" onclick = "fnChoiceCourseTime(this, 'A', 'VALLEY', '0632');" class="timeBtn golfResvCourseTime">06:32</a>
            'fnChoiceCourseTime(this,' 가 onclick 에 포함되어있는 a 태그를 찾는다 
            그것들을 Collection에 추가한다.
            */
            IReadOnlyCollection<IWebElement> HourMinuteTagsCollection = driver.FindElements(By.XPath("//a[contains(@onclick, 'fnChoiceCourseTime(this,')]"));

            foreach (var HourMinuteTag in HourMinuteTagsCollection)
            {
                //onClickVaue 는 onclick 시 호출되는자바스크립트 함수를 문자열로 나타낸것이다.
                string onClickValue = HourMinuteTag.GetAttribute("onclick");

                //숫자부분은 항상 끝에서 7번째 에서부터 4글자이므로 이를 이용한다.
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
            //jsExecutor.ExecuteScript("fnReservation()"); // 테스트할때는 실제 등록되는걸 막기위해 주석을 할것
        }

        private void CancleBooking(IWebDriver driver)
        {
            driver.Quit();
            TotalBrowservolumeLable.Text = "현재 실행중인 브라우저 수 : " + (--TotalBrowservolume).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //chrome 브라우저와 소통 하게 해주는 ChromeDriver
            IWebDriver driver = new ChromeDriver();
            /* 주석 : `IJavaScriptExecutor` 인터페이스를 사용하여 Selenium WebDriver에서 JavaScript 코드를 실행할 수 있습니다.
             이 인터페이스는 테스트 자동화 중 웹 페이지의 자바스크립트 환경에 직접 접근하여 코드를 실행하게 해줍니다.
             예를 들어, 페이지에 보이지 않는 요소를 클릭하거나, 페이지의 DOM을 직접 조작하는 등의 작업을 할 수 있습니다.
             이를 통해, 테스트 시나리오에서 요구하는 복잡한 웹 상호작용을 구현할 수 있습니다. 
            */
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            int browserNumber = BrowserNumber++; // 브라우저에 배정된 번호
            TotalBrowservolumeLable.Text =  "현재 실행중인 브라우저 수 : " + (++TotalBrowservolume).ToString();

            setStatusLabe("로그인중", browserNumber);
            LoginState(driver, jsExecutor);

            try
            {
                setStatusLabe("부킹 페이지로 이동 후 날자 및 시간 선택중", browserNumber);
                GoToReservationAndSelectMonthDateTime(driver, jsExecutor);
            }
            catch(UnhandledAlertException)
            {
                CancleBooking(driver);
                setStatusLabe(" 종료됨 : 날자가 올바르게 입력되었는지 다시 확인해주세요", browserNumber);
                return;
            }

            try
            {
                setStatusLabe("날자, 시간 확인후 실제 부킹 등록중", browserNumber);
                ExecuteBooking(driver, jsExecutor);
            }
            catch (WebDriverException) 
            { 
                setStatusLabe("해당 날자 예약 가능한 시간 없어 종료", browserNumber);
            }
        }

        private void setStatusLabe(string status, int browserNumber)
        {
            statusLabe.Text = browserNumber.ToString() + "번 브라우저 상태 : " + status;
        }

        private void setStatusLabe(string status)
        {
            statusLabe.Text = "상태 : " + status;
        }
    }
}
