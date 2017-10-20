using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

namespace PathOfExileNetWorth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        OverlayForm frm = new OverlayForm();
        private bool frmShown = true;

        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker tabsWorker = new BackgroundWorker();
        private readonly BackgroundWorker charactersWorker = new BackgroundWorker();
        private readonly BackgroundWorker itemsWorker = new BackgroundWorker();

        private Dictionary<string, bool> activeCharacters = new Dictionary<string, bool>();
        private Dictionary<string, bool> activeStashTabs = new Dictionary<string, bool>();

        private List<CharacterOnForm> charsOnForm = new List<CharacterOnForm>();
        private List<StashTabOnForm> stashTabsOnForm = new List<StashTabOnForm>();
        private List<ItemOnForm> items = new List<ItemOnForm>();

        private bool itemsRefreshing = false;
        private bool firstTimePrices = true;
        private bool firstTimeItems = true;

        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

        float res = 0.0f;

        public MainWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;

            ni.Icon = new System.Drawing.Icon("raccoonFace.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                };
            ni.Text = "PoE NetWorth. Double-click to show the app when minimized";

            List<League> leagues = DataManagement.processJsonFromApi<List<League>>(Properties.Settings.Default.poeApiActiveLeagues);
            leagues.RemoveAll(DataManagement.LeagueStartsWithSSF);
            foreach (League l in leagues)
            {
                cBLeague.Items.Add(l.id);
            }

            LoadUserSettings();

            if (frmShown) { frm.Show(); }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized) { Hide(); }

            base.OnStateChanged(e);
        }

        private void LoadUserSettings()
        {
            tbPOESESSID.Text = Properties.Settings.Default.POESESSID;
            tbUsername.Text = Properties.Settings.Default.PoeUsername;
            cBLeague.Text = Properties.Settings.Default.League;
            tbRefreshTime.Text = Properties.Settings.Default.refreshTime.ToString();
            tbItemRefreshTime.Text = Properties.Settings.Default.itemsRefreshTime.ToString();

            rBtnChaosPricing.IsChecked = !Properties.Settings.Default.pricingInExalted;
            rBtnExaltedPricing.IsChecked = Properties.Settings.Default.pricingInExalted;
        } //TODO: move

        private void FetchCharacters()
        {
            charactersWorker.DoWork += charactersWorker_DoWork;
            charactersWorker.RunWorkerCompleted += charactersWorker_RunWorkerCompleted;
            charactersWorker.RunWorkerAsync();
        } //TODO: move

        private void SaveCharacters()
        {
            Dictionary<string, bool> ac = new Dictionary<string, bool>();
            foreach (CharacterOnForm cof in charsOnForm)
            {
                ac.Add(cof.name, cof.active);
            }

            DataManagement.SaveJson(ac, "active_characters.json"); //TODO: handle this moronic case...
        }//TODO: move

        private void FetchTabs()
        {
            tabsWorker.DoWork += tabsWorker_DoWork;
            tabsWorker.RunWorkerCompleted += tabsWorker_RunWorkerCompleted;
            tabsWorker.RunWorkerAsync();
        } //TODO: move

        private void SaveTabs()
        {
            Dictionary<string, bool> ast = new Dictionary<string, bool>();
            foreach (StashTabOnForm sof in stashTabsOnForm)
            {
                ast.Add(sof.id, sof.active);
            }

            DataManagement.SaveJson(ast, "active_stash_tabs.json"); //TODO: handle this moronic case...
        }//TODO: move

        private void btnStashTabs_Click(object sender, RoutedEventArgs e)
        {
            FetchTabs();
        }

        private void btnFetchCharacters_Click(object sender, RoutedEventArgs e)
        {
            FetchCharacters();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FetchCharacters();
            FetchTabs();            
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if(!firstTimePrices)
                Thread.Sleep(Properties.Settings.Default.refreshTime * 1000);
            firstTimePrices = false;

            lblPricesStatus.Dispatcher.Invoke(() => { lblPricesStatus.Content = "Fetching Prices..."; });
            string poeNinjaApi = Properties.Settings.Default.NinjaApi;
            string league = "league=" + Properties.Settings.Default.League;
            NinjaCurrencyData fragments = DataManagement.processJsonFromApi<NinjaCurrencyData>(poeNinjaApi + "GetFragmentOverview" + "?" + league);
            NinjaCurrencyData currencies = DataManagement.processJsonFromApi<NinjaCurrencyData>(poeNinjaApi + "GetCurrencyOverview" + "?" + league);
            NinjaItemData essences = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetEssenceOverview" + "?" + league);
            NinjaItemData divCards = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetDivinationCardsOverview" + "?" + league);
            NinjaItemData prophecies = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetProphecyOverview" + "?" + league);
            NinjaItemData uniqueMap = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueMapOverview" + "?" + league);
            NinjaItemData uniqueJewels = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueJewelOverview" + "?" + league);
            NinjaItemData uniqueFlasks = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueFlaskOverview" + "?" + league);
            NinjaItemData uniqueWeapons = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueWeaponOverview" + "?" + league);
            NinjaItemData uniqueArmours = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueArmourOverview" + "?" + league);
            NinjaItemData uniqueAccessories = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetUniqueAccessoryOverview" + "?" + league);
            NinjaItemData maps = DataManagement.processJsonFromApi<NinjaItemData>(poeNinjaApi + "GetMapOverview" + "?" + league);

            List<NinjaCurrencyData> ccyPrices = new List<NinjaCurrencyData>();
            ccyPrices.Add(fragments);
            ccyPrices.Add(currencies);

            List<NinjaItemData> itemPrices = new List<NinjaItemData>();
            itemPrices.Add(essences);
            itemPrices.Add(divCards);
            itemPrices.Add(prophecies);
            itemPrices.Add(uniqueMap);
            itemPrices.Add(uniqueJewels);
            itemPrices.Add(uniqueFlasks);
            itemPrices.Add(uniqueWeapons);
            itemPrices.Add(uniqueArmours);
            itemPrices.Add(uniqueAccessories);
            itemPrices.Add(maps);

            NinjaPrices.RefreshPrices(ccyPrices, itemPrices);

            worker.DoWork -= worker_DoWork;
        } //TODO: move
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgNinjaPrices.ItemsSource = null;
            dgNinjaPrices.ItemsSource = NinjaPrices.priceOf;
            lblPricesStatus.Content = "Prices fetched! Restarting in " + Properties.Settings.Default.refreshTime + " seconds.";

            if (NinjaPrices.isRefreshingActive && !worker.IsBusy)
            {
                worker.DoWork += worker_DoWork;
                worker.RunWorkerAsync();
            }
            else
            {
                worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
            }
        } //TODO: move

        private void tabsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            lblTabStatus.Dispatcher.Invoke(() =>
            {
                tbPOESESSID.IsEnabled = false;
                tbUsername.IsEnabled = false;
                cBLeague.IsEnabled = false;
                lblTabStatus.Content = "Fetching Tabs...";
            });

            stashTabsOnForm.Clear();
            Stash stash = DataManagement.processJsonFromApi<Stash>(Properties.Settings.Default.poeApiCharacterWindow + "get-stash-items?accountName=" + Properties.Settings.Default.PoeUsername + "&league=" + Properties.Settings.Default.League + "&tabIndex=0&tabs=1"); //TODO: this needs work
            foreach (Tab t in stash.tabs)
            {
                StashTabOnForm sof = new StashTabOnForm(t);
                stashTabsOnForm.Add(sof);
            }

            activeStashTabs = DataManagement.LoadJson<Dictionary<string, bool>>("active_stash_tabs.json"); //TODO: handle this moronic location...

            foreach (StashTabOnForm sof in stashTabsOnForm)
            {
                sof.ActivateDeactivateStashTab(activeStashTabs);
            }

            tabsWorker.DoWork -= tabsWorker_DoWork;
        } //TODO: move
        private void tabsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgStashTabs.ItemsSource = null;
            dgStashTabs.ItemsSource = stashTabsOnForm;
            dgStashTabs.Columns[3].MaxWidth = 0.0f; //this hides ID column
            lblTabStatus.Content = "Tabs Fetched at " + DateTime.Now.ToShortTimeString();

            tabsWorker.RunWorkerCompleted -= tabsWorker_RunWorkerCompleted;

            if (!charactersWorker.IsBusy)
            {
                tbPOESESSID.IsEnabled = true;
                tbUsername.IsEnabled = true;
                cBLeague.IsEnabled = true;
            }

            SaveTabs();
        } //TODO: move

        private void charactersWorker_DoWork(object sender, DoWorkEventArgs e)
        {            
            lblCharactersStatus.Dispatcher.Invoke(() =>
            {
                tbPOESESSID.IsEnabled = false;
                tbUsername.IsEnabled = false;
                cBLeague.IsEnabled = false;
                lblCharactersStatus.Content = "Fetching Characters...";
            });

            charsOnForm.Clear();

            List<Character> chars = DataManagement.processJsonFromApi<List<Character>>(Properties.Settings.Default.poeApiCharacterWindow + "get-characters?account=" + Properties.Settings.Default.PoeUsername);
            foreach (Character c in chars)
            {
                if (c.league == Properties.Settings.Default.League)//TODO: this might need to change...
                {
                    CharacterOnForm cof = new CharacterOnForm(c);
                    charsOnForm.Add(cof);
                }
            }

            activeCharacters = DataManagement.LoadJson<Dictionary<string, bool>>("active_characters.json"); //TODO: handle this moronic location...

            foreach (CharacterOnForm cof in charsOnForm)
            {
                cof.ActivateDeactivateCharacter(activeCharacters);
            }

            charactersWorker.DoWork -= charactersWorker_DoWork;
        } //TODO: move
        private void charactersWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgCharacters.ItemsSource = null;
            dgCharacters.ItemsSource = charsOnForm;
            dgCharacters.Columns[0].IsReadOnly = true; //make columns readonly
            dgCharacters.Columns[1].IsReadOnly = true;
            dgCharacters.Columns[2].IsReadOnly = true;
            lblCharactersStatus.Content = "Characters Fetched at " + DateTime.Now.ToShortTimeString();

            charactersWorker.RunWorkerCompleted -= charactersWorker_RunWorkerCompleted;

            if (!tabsWorker.IsBusy)
            {
                tbPOESESSID.IsEnabled = true;
                tbUsername.IsEnabled = true;
                cBLeague.IsEnabled = true;
            }
            SaveCharacters();
        } //TODO: move

        private void itemsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!firstTimeItems) { Thread.Sleep(Properties.Settings.Default.itemsRefreshTime * 1000); } else { Thread.Sleep(3000); }
                
            firstTimeItems = false;

            items.Clear();

            Dictionary<string, bool> chars = new Dictionary<string, bool>();
            Dictionary<string, bool> stashes = new Dictionary<string, bool>();
            chars = DataManagement.LoadJson<Dictionary<string, bool>>("active_characters.json");
            stashes = DataManagement.LoadJson<Dictionary<string, bool>>("active_stash_tabs.json");

            //item source: characters (inventory + skill tree)
            lblItems.Dispatcher.Invoke(() => { lblItems.Content = "Calculating Character Networth..."; });
            foreach (KeyValuePair<string, bool> kvp in chars)
            {
                if (kvp.Value)
                {
                    CharacterInventory c = DataManagement.processJsonFromApi<CharacterInventory>("https://pathofexile.com/character-window/get-items?eqData=false&character=" + kvp.Key); //TODO: move to app settings...
                    foreach (Item i in c.items)
                    {
                        ItemOnForm iof = new ItemOnForm(i, NinjaPrices.priceOf);
                        items.Add(iof);
                    }

                    SkillTree s = DataManagement.processJsonFromApi<SkillTree>("http://www.pathofexile.com/character-window/get-passive-skills?eqData=false&character=" + kvp.Key);
                    foreach (Item i in s.items)
                    {
                        ItemOnForm iof = new ItemOnForm(i, NinjaPrices.priceOf);
                        items.Add(iof);
                    }
                }
            }

            lblItems.Dispatcher.Invoke(() => { lblItems.Content = "Calculating Stash Tabs Networth..."; });
            Stash stash = DataManagement.processJsonFromApi<Stash>(Properties.Settings.Default.poeApiCharacterWindow + "get-stash-items?accountName=" + Properties.Settings.Default.PoeUsername + "&league=" + Properties.Settings.Default.League + "&tabIndex=-1&tabs=1"); //TODO: this needs work

            foreach (KeyValuePair<string, bool> kvp in stashes)
            {
                if (kvp.Value)
                {
                    int tabIndex = 1000000; //this sould yield an error from the api
                    foreach (Tab t in stash.tabs)
                    {
                        if (kvp.Key == t.id) { tabIndex = t.i; }
                    }

                    Stash s = DataManagement.processJsonFromApi<Stash>(Properties.Settings.Default.poeApiCharacterWindow + "get-stash-items?accountName=" + Properties.Settings.Default.PoeUsername + "&league=" + Properties.Settings.Default.League + "&tabIndex=" + tabIndex.ToString() + "&tabs=0");
                    foreach (Item i in s.items)
                    {
                        ItemOnForm iof = new ItemOnForm(i, NinjaPrices.priceOf);
                        items.Add(iof);
                    }
                }
            }
            itemsWorker.DoWork -= itemsWorker_DoWork;
        }
        private void itemsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = items;

            res = 0.0f;
            foreach (ItemOnForm i in items)
            {
                res += i.numberOfItems * i.price;
            }

            RefreshOverlayNetworth();            

            lblItems.Content = "Pricing Complete! Restarting in " + Properties.Settings.Default.itemsRefreshTime + " seconds.";

            if(itemsRefreshing && !itemsWorker.IsBusy)
            {
                itemsWorker.DoWork += itemsWorker_DoWork;
                itemsWorker.RunWorkerAsync();
            }
            else
            {
                itemsWorker.RunWorkerCompleted -= itemsWorker_RunWorkerCompleted;
            }
        }

        private void RefreshOverlayNetworth()
        {
            if (Properties.Settings.Default.pricingInExalted)
            {
                try
                {
                    frm.NetWorthLabelText = Math.Round(res / NinjaPrices.priceOf["Exalted Orb_0"], 2).ToString() + " exa";
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    frm.NetWorthLabelText = "0 exa";
                }
            }
            else
            {
                frm.NetWorthLabelText = Math.Round(res, 0).ToString() + " chaos";
            }
        }

        private void cBLeague_DropDownClosed(object sender, EventArgs e)
        {
            Properties.Settings.Default.League = cBLeague.Text;
            Properties.Settings.Default.Save();

            FetchCharacters();
            FetchTabs();
        }

        private void btnShowOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (!frmShown) { frm.Show(); } else { frm.Hide(); }
            frmShown = !frmShown;

            cBoxLockOverlay.IsEnabled = !cBoxLockOverlay.IsEnabled;
        }

        private void cBoxLockOverlay_Click_1(object sender, RoutedEventArgs e)
        {
            if((bool)cBoxLockOverlay.IsChecked)
            {
                frm.SetWindowTransparent();
            }
            else
            {
                frm.SetWindowOpaque();
            }
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if(Properties.Settings.Default.POESESSID == "" || Properties.Settings.Default.PoeUsername == "")
            {
                MessageBox.Show("Please enter POESESSID and/or Username", "Information", MessageBoxButton.OK);
                return;
            }
            NinjaPrices.isRefreshingActive = !NinjaPrices.isRefreshingActive;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            if (NinjaPrices.isRefreshingActive && !worker.IsBusy)
            {
                firstTimePrices = true;
                worker.RunWorkerAsync();
            }

            itemsRefreshing = !itemsRefreshing;
            itemsWorker.DoWork += itemsWorker_DoWork;
            itemsWorker.RunWorkerCompleted += itemsWorker_RunWorkerCompleted;

            if (itemsRefreshing && !itemsWorker.IsBusy)
            {
                firstTimeItems = true;
                itemsWorker.RunWorkerAsync();
            }

            if ((string)btnStartStop.Content == "START!") { btnStartStop.Content = "Stop!"; } else { btnStartStop.Content = "START!"; }

            tbPOESESSID.IsEnabled = !tbPOESESSID.IsEnabled;
            tbUsername.IsEnabled = !tbUsername.IsEnabled;
            cBLeague.IsEnabled = !cBLeague.IsEnabled;
            btnFetchCharacters.IsEnabled = !btnFetchCharacters.IsEnabled;
            btnStashTabs.IsEnabled = !btnStashTabs.IsEnabled;
        }

        private void rBtnChaosPricing_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.pricingInExalted = (bool)rBtnExaltedPricing.IsChecked;
            Properties.Settings.Default.Save();

            //float x;
            //if(float.TryParse(frm.NetWorthLabelText,out x))
            //{
                RefreshOverlayNetworth();
            //}
        }
        
        private void Window_Closed(object sender, EventArgs e)
        {
            ni.Dispose();
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ni.Dispose();
                    frm.Dispose();
                    worker.Dispose();
                    tabsWorker.Dispose();
                    itemsWorker.Dispose();
                    charactersWorker.Dispose();
                }
                
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MainWindow() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            if(btnShowDetails.Content.ToString()=="More >>")
            {
                btnShowDetails.Content = "<< Less";
                Width = 1440;
            }
            else
            {
                btnShowDetails.Content = "More >>";
                Width = 700;
            }
        }

        private void btnPoeSessIdHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://code.google.com/archive/p/procurement/wikis/LoginWithSessionID.wiki");           
        }

        private void SaveUsername()
        {
            Properties.Settings.Default.PoeUsername = tbUsername.Text;
            Properties.Settings.Default.Save();
        }
        private void SavePOESESSID()
        {
            Properties.Settings.Default.POESESSID = tbPOESESSID.Text;
            Properties.Settings.Default.Save();
        } 
        private void SaveRefreshTime()
        {
            int x;
            if (int.TryParse(tbRefreshTime.Text, out x)) { Properties.Settings.Default.refreshTime = x; }
            Properties.Settings.Default.Save();
        }
        private void SaveItemRefreshTime()
        {
            int x;
            if (int.TryParse(tbItemRefreshTime.Text, out x)) { Properties.Settings.Default.itemsRefreshTime = x; }
            Properties.Settings.Default.Save();
        }

        private void tbUsername_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key==Key.Tab)
            {
                SaveUsername();
            }
        }
        private void tbPOESESSID_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter || e.Key == Key.Tab)
            {
                SavePOESESSID();
            }
        }
        private void tbRefreshTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                SaveRefreshTime();
            }
        }
        private void tbItemRefreshTime_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                SaveItemRefreshTime();
            }
        }

        private void tbUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveUsername();
        }
        private void tbPOESESSID_LostFocus(object sender, RoutedEventArgs e)
        {
            SavePOESESSID();
        }
        private void tbRefreshTime_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveRefreshTime();
        }
        private void tbItemRefreshTime_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveItemRefreshTime();
        }

        private void dgCharacters_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            dgCharacters.CommitEdit();
        }
        private void dgCharacters_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (dgCharacters.SelectedItem != null)
            {
                (sender as DataGrid).CellEditEnding -= dgCharacters_CellEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();
                (sender as DataGrid).CellEditEnding += dgCharacters_CellEditEnding;
            }
            else return;
            SaveCharacters();
        }

        #region SingleClickCheckbox
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }
        private void DataGridCell_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DataGridCell cell = sender as DataGridCell;
            GridColumnFastEdit(cell, e);
        }
        private static void GridColumnFastEdit(DataGridCell cell, RoutedEventArgs e)
        {
            if (cell == null || cell.IsEditing || cell.IsReadOnly)
                return;

            DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
            if (dataGrid == null)
                return;

            if (!cell.IsFocused)
            {
                cell.Focus();
            }

            if (cell.Content is CheckBox)
            {
                if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
                {
                    if (!cell.IsSelected)
                        cell.IsSelected = true;
                }
                else
                {
                    DataGridRow row = FindVisualParent<DataGridRow>(cell);
                    if (row != null && !row.IsSelected)
                    {
                        row.IsSelected = true;
                    }
                }
            }
            else
            {
                ComboBox cb = cell.Content as ComboBox;
                if (cb != null)
                {
                    //DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
                    dataGrid.BeginEdit(e);
                    cell.Dispatcher.Invoke(
                     System.Windows.Threading.DispatcherPriority.Background,
                     new Action(delegate { }));
                    cb.IsDropDownOpen = true;
                }
            }
        }
        private static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }
        #endregion

        private void dgStashTabs_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            dgStashTabs.CommitEdit();
        }
        private void dgStashTabs_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (dgStashTabs.SelectedItem != null)
            {
                (sender as DataGrid).CellEditEnding -= dgStashTabs_CellEditEnding;
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).CommitEdit();
                (sender as DataGrid).Items.Refresh();
                (sender as DataGrid).CellEditEnding += dgStashTabs_CellEditEnding;
            }
            else return;
            SaveTabs();
        }
    }
}