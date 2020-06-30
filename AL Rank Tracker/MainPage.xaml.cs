using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace AL_Rank_Tracker
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Boolean debug_enabled = false;
        private String platform = "origin";
        private String user = "";
        dynamic json;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Refresh_Button_Clicked(object sender, EventArgs e)
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                Username_Frame.IsVisible = false;
                Username_EndFrame.IsVisible = false;
                Rank_Player.Text = "";
                Rank_Player_Dup.Text = "";
                Setup_Form();
                Update_Form(user);
            }
            else
            {
                await DisplayAlert("Connection Error", "An internet connection is required to use this app. Please enable data or connect to a wifi network and try again.", "OK");
            }


        }

        private async void Enter_Button_Clicked(object sender, EventArgs e)
        {

            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                try
                {

                    user = Username_Field.Text;

                    if (user.ToLower().Equals("alpha enable debug button"))
                    {
                        await DisplayAlert("Debug Enabled", "If you found this accidentally, Good Job <3 !", "OK");
                        Debug_Button.IsVisible = true;
                        return;
                    }

                    if (user.ToLower().Equals("alpha disable debug button"))
                    {
                        await DisplayAlert("Debug Disabled", "If you found this accidentally, Good Job <3 !", "OK");
                        Debug_Button.IsVisible = false;
                        return;
                    }

                    if (user.Length <= 0)
                    {
                        await DisplayAlert("Search Error", "Invaild Username: Name cannot be blank", "OK");
                        return;
                    };

                    if (debug_enabled)
                    {
                        await DisplayAlert("Debug Stats", "User Length: " + user.Length, "OK");
                        await Search_username(Username_Field.Text);
                        Debug_Print(Username_Field.Text + "\n" + platform + "\n\n" + json);
                    }
                    else
                    {
                        Setup_Form();
                        Update_Form(user);
                    }
                }
                catch (NullReferenceException)
                {
                    Username_Frame.IsVisible = false;
                    Username_EndFrame.IsVisible = false;
                    await DisplayAlert("Search Error", "Invaild Username: Name cannot be blank.", "OK");
                    return;
                }
                catch (Exception ev)
                {
                    Username_Frame.IsVisible = false;
                    Username_EndFrame.IsVisible = false;
                    await DisplayAlert(ev.Source, ev.Message + " Please screenshot and report this error to https://forms.gle/zEix9oiqRtkwrp7y5", "OK");
                }
            }
            else
            {
                await DisplayAlert("Connection Error", "An internet connection is required to use this app. Please enable data or connect to a wifi network and try again.", "OK");
            }

        }

        public async void Update_Form(String user)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    //Api handler
                    client.DefaultRequestHeaders.Add("TRN-Api-Key", "");

                    //Response from http request
                    var response = await client.GetAsync($"https://public-api.tracker.gg/v2/apex/standard/profile/{platform}/{user}");
                    //Console.WriteLine(response);

                    //json = response;

                    //content strign conversion
                    var content = await response.Content.ReadAsStringAsync();

                    //JSON conversion to string data
                    dynamic item = Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                    try
                    {
                        Rank_Player.Text = item.data.platformInfo.platformUserId;
                        Rank_Player_Dup.Text = item.data.platformInfo.platformUserId;
                    }
                    catch (RuntimeBinderException)
                    {
                        await DisplayAlert("Search Error", $"No User was found with the username: {user}", "OK");
                        Username_Frame.IsVisible = false;
                        Username_EndFrame.IsVisible = false;
                        return;
                    }


                    try
                    {
                        Rank_Value.Text = item.data.segments[0].stats.rankScore.value;
                        Rank_Title.Text = item.data.segments[0].stats.rankScore.metadata.rankName;
                        String icon = item.data.segments[0].stats.rankScore.metadata.iconUrl;
                        Rank_Img.Source = ImageSource.FromUri(new Uri(icon));
                    }
                    catch (RuntimeBinderException)
                    {
                        await DisplayAlert("Search Error", "No Ranked Data on server to be Displayed.", "OK");
                        Username_Frame.IsVisible = false;
                        Username_EndFrame.IsVisible = false;
                        return;
                    }
                }
            }
            catch (NullReferenceException nullRef)
            {
                Username_Frame.IsVisible = false;
                Username_EndFrame.IsVisible = false;
                await DisplayAlert("No Results Found", "Please check input and try again, if problem persists report at: https://forms.gle/zEix9oiqRtkwrp7y5.", "OK");
                return;
            }
            catch (Exception ev)
            {
                Username_Frame.IsVisible = false;
                Username_EndFrame.IsVisible = false;
                await DisplayAlert(ev.HelpLink, ev.Message + " Please screenshot and report this error to https://forms.gle/zEix9oiqRtkwrp7y5", "OK");
            }
        }

        private void Debug_Print(String e)
        {
            Dialogue.IsVisible = false;
            Debug_Stack.IsVisible = true;
            Debug_Frame.IsVisible = true;
            debug_text.Text = e;
        }

        private void Setup_Form()
        {
            Username_Frame.IsVisible = true;
            Username_EndFrame.IsVisible = true;
            Rank_Player.Text = "Loading....";
            Rank_Player_Dup.Text = "Loading....";
        }

        private async Task Search_username(String user)
        {
            try
            {
                using (var client = new HttpClient())
                {

                    //Api handler
                    client.DefaultRequestHeaders.Add("TRN-Api-Key", "83172a11-079f-4559-b571-b86da2205a9e");

                    //Response from http request
                    var response = await client.GetAsync($"https://public-api.tracker.gg/v2/apex/standard/profile/{platform}/{user}");
                    //Console.WriteLine(response);

                    //json = response;

                    //content strign conversion
                    var content = await response.Content.ReadAsStringAsync();

                    //JSON conversion to string data
                    dynamic item = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                    json = item.data.platformInfo.platformUserId + " = Found! Connecion Established.";
                }
            }
            catch (NullReferenceException nullRef)
            {
                await DisplayAlert("No Results Found", "Please check input and try again, if problem persists report at: https://forms.gle/zEix9oiqRtkwrp7y5.", "OK");
                return;
            }
            catch (Exception ev)
            {
                await DisplayAlert(ev.HelpLink, ev.Message + " Please screenshot and report this error to https://forms.gle/zEix9oiqRtkwrp7y5", "OK");
            }
        }

        private void Debug_Button_Clicked(object sender, EventArgs e)
        {
            if (debug_enabled)
            {
                Dialogue.IsVisible = true;
                Debug_Stack.IsVisible = false;
                Debug_Frame.IsVisible = false;
                debug_text.Text = "";
                debug_enabled = false;
                Debug_Button.TextColor = Color.Black;
            }
            else
            {
                debug_enabled = true;
                Debug_Button.TextColor = Color.Green;
            }
        }

        private void PC_Button_Clicked(object sender, EventArgs e)
        {
            PS4_Button.TextColor = Color.Black;
            Xbox_Button.TextColor = Color.Black;
            PC_Button.TextColor = Color.Green;
            platform = "origin";
        }

        private void Xbox_Button_Clicked(object sender, EventArgs e)
        {
            PS4_Button.TextColor = Color.Black;
            Xbox_Button.TextColor = Color.Green;
            PC_Button.TextColor = Color.Black;
            platform = "xbl";
        }

        private void PS4_Button_Clicked(object sender, EventArgs e)
        {
            PS4_Button.TextColor = Color.Green;
            Xbox_Button.TextColor = Color.Black;
            PC_Button.TextColor = Color.Black;
            platform = "psn";
        }
    }
}
