﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Sog_SGreader
{
    public partial class FrmMain : Form
    {
        private readonly Sog_Player playerObject = new Sog_Player();

        public FrmMain(string sFilePath)
        {
            InitializeComponent();    //Initializing elements from the Designer
            InitElements(); //Initializing elements from this file
            txtConsole.AppendText("Support me by giving the Repository a star on Github \r\n");
            txtConsole.AppendText("https://github.com/tolik518/SoG_SGreader \r\n");
            txtConsole.AppendText("________________________________________\r\n");
            if (File.Exists(sFilePath))
            {
                LoadSaveGame(sFilePath);
            } 
            else
            {
                txtConsole.AppendText("Savefile with the following path doesnt exist: " + sFilePath + "\r\n");
            }
        }
        //TODO: When a file is beeing opened, we need to reset als variables
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtConsole.Text = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Secrets of Grindea\Characters"),
                Filter = "SoG Savegame (*.cha)|*.cha",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadSaveGame(openFileDialog1.FileName);
            }
        }

        internal void LoadSaveGame(string sFilePath)
        {
            txtConsole.AppendText(sFilePath);
            ReadData(sFilePath);
            saveToolStripMenuItem.Enabled = true;

            InitFields();
            PopulateFileds();
        }

        private readonly ComboBox[] cbQuickslot = new ComboBox[10];
        private readonly ComboBox[] cbQuickslotType = new ComboBox[10];

        //TODO: Check out if there is a way to show the items in the designer 
        private void InitElements() //  Designer Items
        {
            int iQuickslotYpos = 262;
            for (int i = 9; i >= 0; i--) // We need 10 comboBoxes for the quickslots and for the types
            {                            // Its nice to have them accessible through an index
                cbQuickslot[i] = new ComboBox
                {
                    FormattingEnabled = true,
                    Location = new Point(180, iQuickslotYpos),
                    Name = $"cbQuickslot[{i}]",
                    Size = new Size(152, 21),
                    Enabled = false
                };

                cbQuickslotType[i] = new ComboBox
                {
                    FormattingEnabled = true,
                    Location = new Point(94, iQuickslotYpos),
                    Name = $"cbQuickslotType[{i}]",
                    Size = new Size(79, 21),
                    Enabled = false
                };
                groupBox3.Controls.Add(cbQuickslotType[i]);
                groupBox3.Controls.Add(cbQuickslot[i]);
                cbQuickslotType[i].SelectedIndexChanged += new System.EventHandler(QuickslotType_SelectedIndexChanged);

                iQuickslotYpos -= 27;
            }
        }

        //this method is responsible to update the content of the Quckslot combobox depending on what
        //was selected in the Type combobox
        private void QuickslotType_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i != 10; i++)
            {
                if (cbQuickslotType[i].Text == "Sog_Items")
                {
                    cbQuickslot[i].DataSource = Enum.GetValues(typeof(Sog_Items));
                }
                else if (cbQuickslotType[i].Text == "Sog_Spells")
                {
                    cbQuickslot[i].DataSource = Enum.GetValues(typeof(Sog_Skills));
                }
                else
                {
                    cbQuickslot[i].DataSource = null;
                }
                cbQuickslot[i].Text = (playerObject.quickslots[i]).ToString();
            }
        }

        private void InitFields()
        {
            var items = Enum.GetNames(typeof(Sog_Items));

            //Enum.GetValues(typeof((Enum)Enum.GetNames(typeof(Sog_Items)).Where(item => item.StartsWith("_Hat_"))));
            cbHat.DataSource = items.Where(item => item.StartsWith("_Hat_") || item == "Null").ToArray(); //TODO

            cbFacegear.DataSource = items.Where(item => item.StartsWith("_Facegear_") || item == "Null").ToArray();

            cbWeapon.DataSource = items.Where(item => item.StartsWith("_OneHanded_") ||
                                                      item.StartsWith("_TwoHanded_") ||
                                                      item.StartsWith("_Bow_") || item == "Null").ToArray();

            cbShield.DataSource = items.Where(item => item.StartsWith("_Shield_") || item == "Null").ToArray();

            cbArmor.DataSource = items.Where(item => item.StartsWith("_Armor_") || item == "Null").ToArray();

            cbShoes.DataSource = items.Where(item => item.StartsWith("_Shoes_") || item == "Null").ToArray();

            cbAccessory1.DataSource = items.Where(item => item.StartsWith("_Accessory_") || item == "Null").ToArray();

            cbAccessory2.DataSource = items.Where(item => item.StartsWith("_Accessory_") || item == "Null").ToArray();

            cbStyleHat.DataSource = items.Where(item => item.StartsWith("_Hat_") || item == "Null").ToArray();

            cbStyleFacegear.DataSource = items.Where(item => item.StartsWith("_Facegear_") || item == "Null").ToArray();

            cbStyleWeapon.DataSource = items.Where(item => item.StartsWith("_OneHanded_") ||
                                                           item.StartsWith("_TwoHanded_") ||
                                                           item.StartsWith("_Bow_") || item == "Null").ToArray();

            cbStyleShield.DataSource = items.Where(item => item.StartsWith("_Shield_") || item == "Null").ToArray();

            //TODO: I need to check if the quickslotsType field changes to fill out the fields with new items
            for (int i = 0; i != 10; i++)
            {
                if (playerObject.quickslots[i].GetType() == typeof(Sog_Items))
                {
                    cbQuickslot[i].DataSource = items;
                }
                else if (playerObject.quickslots[i].GetType() == typeof(Sog_Skills))
                {
                    cbQuickslot[i].DataSource = Enum.GetValues(typeof(Sog_Skills));
                }
                cbQuickslotType[i].DataSource = new string[] { 
                    "Sog_Items", "Sog_Spells", "Int32" 
                };
            }
            cbSelectedItem.DataSource = items;
        }

        private void PopulateFileds()
        {
            txtNickname.Text = playerObject.Nickname;

            cbHat.Text = ((Sog_Items)playerObject.equip.Hat).ToString();
            cbFacegear.Text = ((Sog_Items)playerObject.equip.Facegear).ToString();
            cbWeapon.Text = ((Sog_Items)playerObject.equip.Weapon).ToString();
            cbShield.Text = ((Sog_Items)playerObject.equip.Shield).ToString();
            cbArmor.Text = ((Sog_Items)playerObject.equip.Armor).ToString();
            cbShoes.Text = ((Sog_Items)playerObject.equip.Shoes).ToString();

            cbAccessory1.Text = ((Sog_Items)playerObject.equip.Accessory1).ToString();
            cbAccessory2.Text = ((Sog_Items)playerObject.equip.Accessory2).ToString();

            cbStyleHat.Text = ((Sog_Items)playerObject.style.Hat).ToString();
            cbStyleFacegear.Text = ((Sog_Items)playerObject.style.Facegear).ToString();
            cbStyleWeapon.Text = ((Sog_Items)playerObject.style.Weapon).ToString();
            cbStyleShield.Text = ((Sog_Items)playerObject.style.Shield).ToString();

            for (int i = 0; i < 10; i++)
            {
                cbQuickslot[i].Text = playerObject.quickslots[i].ToString();
                cbQuickslotType[i].Text = (playerObject.quickslots[i].GetType()).Name.ToString();
            }

            btnHairColor.BackColor = ColorTranslator.FromHtml("#" + ((Sog_Colors)playerObject.style.HairColor).ToString().TrimStart('_'));
            // btnSkinColor.BackColor = ColorTranslator.FromHtml("#" + ((SoG_Colors)iHairColor).ToString().TrimStart('_'));
            btnPonchoColor.BackColor = ColorTranslator.FromHtml("#" + ((Sog_Colors)playerObject.style.PonchoColor).ToString().TrimStart('_'));
            btnShirtColor.BackColor = ColorTranslator.FromHtml("#" + ((Sog_Colors)playerObject.style.ShirtColor).ToString().TrimStart('_'));
            btnPantsColor.BackColor = ColorTranslator.FromHtml("#" + ((Sog_Colors)playerObject.style.PantsColor).ToString().TrimStart('_'));

            for (int i = 0; i != playerObject.ItemsCount; i++)
            {
                var vItem = new ListViewItem(
                    new[] { 
                        playerObject.Inventory[i].ItemID.ToString(), 
                        playerObject.Inventory[i].ItemCount.ToString(), 
                        playerObject.Inventory[i].ItemPos.ToString() 
                    }
                );

                lstInventory.Items.Add(vItem);
            }

            numGold.Value = playerObject.Cash;

            numLevel.Value = playerObject.Level;
            numEXPcurrent.Value = playerObject.EXPCurrent;
            numEXPUnknown0.Value = playerObject.EXPUnknown0;
            numEXPUnknown1.Value = playerObject.EXPUnknown1;

            numSkillTalentPoints.Value = playerObject.SkillTalentPoints;
            numSkillSilverPoints.Value = playerObject.SkillSilverPoints;
            numSkillGoldPoints.Value = playerObject.SkillGoldPoints;

            for (int i = 0; i != playerObject.PetsCount; i++)
            {
                var pPet = new ListViewItem(new[] {
                    playerObject.Pets[i].Level.ToString(),
                    playerObject.Pets[i].Nickname.ToString(),
                    playerObject.Pets[i].StatHealth.ToString(),
                    playerObject.Pets[i].StatEnergy.ToString(),
                    playerObject.Pets[i].StatDamage.ToString(),
                    playerObject.Pets[i].StatCrit.ToString(),
                    playerObject.Pets[i].StatSpeed.ToString()
                });

                lstPets.Items.Add(pPet);
            }

            txtTimePlayed.Text = TimeSpan.FromSeconds(playerObject.PlayTimeTotal / 60).ToString(@"d\:hh\:mm\:ss");
            numID.Value = playerObject.UniquePlayerID;
            numBirthdayDay.Value = playerObject.BirthdayDay;
            numBirtdayMonth.Value = playerObject.BirthdayMonth;

            rbMale.Checked = playerObject.style.Sex != 0;
            rbFemale.Checked = playerObject.style.Sex == 0;

            sliderSkillMelee1h0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_OneHanded_Stinger) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_OneHanded_Stinger).SkillLevel : 0;
            sliderSkillMelee1h1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_OneHanded_MillionStabs) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_OneHanded_MillionStabs).SkillLevel : 0;
            sliderSkillMelee1h2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_OneHanded_SpiritSlash) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_OneHanded_SpiritSlash).SkillLevel : 0;
            sliderSkillMelee1h3.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_OneHanded_ShadowClone) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_OneHanded_ShadowClone).SkillLevel : 0;
            sliderSkillMelee1h4.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_OneHanded_QuickCounter) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_OneHanded_QuickCounter).SkillLevel : 0;

            sliderSkillMelee2h0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Overhead) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Overhead).SkillLevel : 0;
            sliderSkillMelee2h1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Spin) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Spin).SkillLevel : 0;
            sliderSkillMelee2h2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Throw) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Throw).SkillLevel : 0;
            sliderSkillMelee2h3.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Smash) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_TwoHanded_Smash).SkillLevel : 0;
            sliderSkillMelee2h4.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Skill_TwoHanded_BerserkMode) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Skill_TwoHanded_BerserkMode).SkillLevel : 0;

            sliderSkillMagicF0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Fire_Fireball) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Fire_Fireball).SkillLevel : 0;
            sliderSkillMagicF1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Fire_Meteor) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Fire_Meteor).SkillLevel : 0;
            sliderSkillMagicF2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Fire_Flamethrower) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Fire_Flamethrower).SkillLevel : 0;

            sliderSkillMagicI0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Ice_IceSpikes) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Ice_IceSpikes).SkillLevel : 0;
            sliderSkillMagicI1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Ice_IceNova) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Ice_IceNova).SkillLevel : 0;
            sliderSkillMagicI2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Ice_FrostyFriend) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Ice_FrostyFriend).SkillLevel : 0;

            sliderSkillMagicE0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Earth_EarthSpike) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Earth_EarthSpike).SkillLevel : 0;
            sliderSkillMagicE1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Earth_SummonPlant) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Earth_SummonPlant).SkillLevel : 0;
            sliderSkillMagicE2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Earth_InsectSwarm) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Earth_InsectSwarm).SkillLevel : 0;

            sliderSkillMagicA0.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Wind_ChainLightning) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Wind_ChainLightning).SkillLevel : 0;
            sliderSkillMagicA1.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Wind_SummonCloud) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Wind_SummonCloud).SkillLevel : 0;
            sliderSkillMagicA2.Value = playerObject.Skills.Any(x => x.SkillID == Sog_Skills._Magic_Wind_StaticTouch) ? playerObject.Skills.Find(x => x.SkillID == Sog_Skills._Magic_Wind_StaticTouch).SkillLevel : 0;
        }

        //TODO: we need to clean all our variables before we load a new file 
        private void InitVariables() { }

        private void GetDataFromFields()
        {
            playerObject.Nickname = txtNickname.Text;
            playerObject.equip.Hat = (int)Enum.Parse(typeof(Sog_Items), cbHat.Text);
            playerObject.equip.Facegear = (int)Enum.Parse(typeof(Sog_Items), cbFacegear.Text);
            playerObject.equip.Weapon = (int)Enum.Parse(typeof(Sog_Items), cbWeapon.Text);
            playerObject.equip.Shield = (int)Enum.Parse(typeof(Sog_Items), cbShield.Text);
            playerObject.equip.Armor = (int)Enum.Parse(typeof(Sog_Items), cbArmor.Text);
            playerObject.equip.Shoes = (int)Enum.Parse(typeof(Sog_Items), cbShoes.Text);

            playerObject.equip.Accessory1 = (int)Enum.Parse(typeof(Sog_Items), cbAccessory1.Text);
            playerObject.equip.Accessory2 = (int)Enum.Parse(typeof(Sog_Items), cbAccessory2.Text);

            playerObject.style.Hat = (int)Enum.Parse(typeof(Sog_Items), cbStyleHat.Text);
            playerObject.style.Facegear = (int)Enum.Parse(typeof(Sog_Items), cbStyleFacegear.Text);
            playerObject.style.Weapon = (int)Enum.Parse(typeof(Sog_Items), cbStyleWeapon.Text);
            playerObject.style.Shield = (int)Enum.Parse(typeof(Sog_Items), cbStyleShield.Text);

            for (int i = 0; i < 10; i++)
            {
                if (cbQuickslotType[i].Text == typeof(Sog_Items).Name.ToString())
                {
                    //playerObject.quickslots[i] = Enum.Parse(typeof(Sog_Items), cbQuickslot[i].Text);
                    playerObject.quickslots[i] = playerObject.quickslots[i];
                }
                else if (cbQuickslotType[i].Text == typeof(Sog_Skills).Name.ToString())
                {
                    //playerObject.quickslots[i] = Enum.Parse(typeof(Sog_Skills), cbQuickslot[i].Text);
                    playerObject.quickslots[i] = playerObject.quickslots[i];
                }
                else
                {
                    playerObject.quickslots[i] = (int)0;
                }
            }

            playerObject.ItemsCount = lstInventory.Items.Count;
            playerObject.Inventory.Clear();

            for (int i = 0; i != lstInventory.Items.Count; i++)
            {
                Sog_Player.Item iitem = new Sog_Player.Item(
                    (Sog_Items)Enum.Parse(typeof(Sog_Items), lstInventory.Items[i].SubItems[0].Text), 
                    Int32.Parse(lstInventory.Items[i].SubItems[1].Text),
                    UInt32.Parse(lstInventory.Items[i].SubItems[2].Text)
                 );

                playerObject.Inventory.Add(iitem);
            }

            playerObject.Level = (Int16)numLevel.Value;
            playerObject.Cash = (int)numGold.Value;
            playerObject.EXPCurrent = (int)numEXPcurrent.Value;
            playerObject.EXPUnknown0 = (int)numEXPUnknown0.Value;
            playerObject.EXPUnknown1 = (int)numEXPUnknown1.Value;

            playerObject.SkillTalentPoints = (Int16)numSkillTalentPoints.Value;
            playerObject.SkillSilverPoints = (Int16)numSkillSilverPoints.Value;
            playerObject.SkillGoldPoints = (Int16)numSkillGoldPoints.Value;

            playerObject.PetsCount = (byte)lstPets.Items.Count;

            for (byte i = 0; i != playerObject.PetsCount; i++)
            {
                playerObject.Pets.Add(new Sog_Player.Pet
                {
                    Type1 = playerObject.Pets[0].Type1,
                    Type2 = playerObject.Pets[0].Type2,
                    Nickname = lstPets.Items[i].SubItems[1].Text,
                    Level = Byte.Parse(lstPets.Items[i].SubItems[0].Text),
                    Skin = playerObject.Pets[0].Skin,

                    StatHealth = UInt16.Parse(lstPets.Items[i].SubItems[2].Text),
                    StatEnergy = UInt16.Parse(lstPets.Items[i].SubItems[3].Text),
                    StatDamage = UInt16.Parse(lstPets.Items[i].SubItems[4].Text),
                    StatCrit = UInt16.Parse(lstPets.Items[i].SubItems[5].Text),
                    StatSpeed = UInt16.Parse(lstPets.Items[i].SubItems[6].Text),

                    StatProgressHealth = playerObject.Pets[0].StatProgressHealth,
                    StatProgressEnergy = playerObject.Pets[0].StatProgressEnergy,
                    StatProgressDamage = playerObject.Pets[0].StatProgressDamage,
                    StatProgressCrit = playerObject.Pets[0].StatProgressCrit,
                    StatProgressSpeed = playerObject.Pets[0].StatProgressSpeed
                });
                playerObject.Pets.RemoveAt(0);
            }

            playerObject.UniquePlayerID = (UInt32)numID.Value;
            playerObject.PlayTimeTotal = (int)(TimeSpan.Parse(txtTimePlayed.Text).TotalSeconds) * 60;
            playerObject.BirthdayDay = (int)numBirthdayDay.Value;
            playerObject.BirthdayMonth = (int)numBirtdayMonth.Value;

            playerObject.style.Sex = rbMale.Checked ? 1 : 0;
        }

        private void LstInventory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstInventory.FocusedItem == null || lstInventory.FocusedItem.Index == -1)
            {
                return;
            }
            cbSelectedItem.Text = lstInventory.Items[lstInventory.FocusedItem.Index].SubItems[0].Text;
            numItemCount.Value = Int32.Parse(lstInventory.Items[lstInventory.FocusedItem.Index].SubItems[1].Text);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sFilename;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save character";
            saveFileDialog1.DefaultExt = ".cha";
            saveFileDialog1.Filter = "Character files (*.cha)|*.cha|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sFilename = saveFileDialog1.FileName.ToString();
                if (!File.Exists(sFilename))
                {
                    FileStream fileStream = File.Create(sFilename); //there should be a better way to do this lol
                    fileStream.Close();
                }

                WriteData(sFilename);
                txtConsole.AppendText("\r\n\r\nFile was saved successfully under: ");
                txtConsole.AppendText("\r\n" + sFilename);
            }
            else
            {
                txtConsole.AppendText("\r\n\r\nFile was NOT saved.");
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.MessageLoop)
            {
                Application.Exit();
            }
            else
            {
                Environment.Exit(1);
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout frmAbout = new FrmAbout();
            frmAbout.ShowDialog();
        }

        private void NumItemCount_ValueChanged(object sender, EventArgs e)
        {
            if (lstInventory.FocusedItem != null)
            {
                lstInventory.Items[lstInventory.FocusedItem.Index].SubItems[1].Text = numItemCount.Value.ToString();
            }
        }

        private void BtnAddItem_Click(object sender, EventArgs e)
        {
            string sSelectedItem = cbSelectedItem.Text;
            uint maxPos = 0;
            for (int i = 0; i != lstInventory.Items.Count; i++)
            {
                if (UInt32.Parse(lstInventory.Items[i].SubItems[2].Text) > maxPos)
                {
                    maxPos = UInt32.Parse(lstInventory.Items[i].SubItems[2].Text);
                }
            }

            if (lstInventory.FindItemWithText(cbSelectedItem.Text) == null) //look if the item already exists; 
            {
                var vItem = new ListViewItem(new[] { cbSelectedItem.Text, numItemCount.Value.ToString(), (++maxPos).ToString() });
                lstInventory.Items.Add(vItem);
                //Sog_Player.Item iitem = new Sog_Player.Item((Sog_Items)Enum.Parse(typeof(Sog_Items), lstInventory.Items[lstInventory.Items.Count - 1].SubItems[0].Text),
                //                                                                 Int32.Parse(lstInventory.Items[lstInventory.Items.Count - 1].SubItems[1].Text),
                //                                                                  UInt32.Parse(lstInventory.Items[lstInventory.Items.Count - 1].SubItems[2].Text));
                //pPlayer.inventory.Add(iitem);
                //pPlayer.InventorySize++;
            }
            lstInventory.EnsureVisible(lstInventory.FindItemWithText(cbSelectedItem.Text).Index);
            lstInventory.Items[lstInventory.FindItemWithText(cbSelectedItem.Text).Index].Selected = true;
            lstInventory.Select();

            cbSelectedItem.Text = sSelectedItem;
        }

        private void ClothingColor_Click(object sender, EventArgs e)
        {
            using (var form = new FrmColorSelect())
            {
                form.ShowDialog();
                string sColor = form.sColor;

                if (!string.IsNullOrEmpty(sColor))
                {
                    ((Control)sender).BackColor = ColorTranslator.FromHtml(sColor);
                    sColor = "_" + form.sColor.TrimStart('#');

                    if (((Control)sender) == btnHairColor)
                    {
                        playerObject.style.HairColor = (byte)(Sog_Colors)Enum.Parse(typeof(Sog_Colors), sColor);
                    }
                    else if (((Control)sender) == btnPonchoColor)
                    {
                        playerObject.style.PonchoColor = (byte)(Sog_Colors)Enum.Parse(typeof(Sog_Colors), sColor);
                    }
                    else if (((Control)sender) == btnShirtColor)
                    {
                        playerObject.style.ShirtColor = (byte)(Sog_Colors)Enum.Parse(typeof(Sog_Colors), sColor);
                    }
                    else if (((Control)sender) == btnPantsColor)
                    {
                        playerObject.style.PantsColor = (byte)(Sog_Colors)Enum.Parse(typeof(Sog_Colors), sColor);
                    }
                }
            }
            ActiveControl = label1; //workarround so the button won't be highlighted anymore
        }

        private void LstPets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPets.FocusedItem == null)
            {
                return;
            }

            int index = lstPets.FocusedItem.Index;
            numPetHP.Value = Int32.Parse(lstPets.Items[index].SubItems[2].Text);
            numPetEnergy.Value = Int32.Parse(lstPets.Items[index].SubItems[3].Text);
            numPetDamage.Value = Int32.Parse(lstPets.Items[index].SubItems[4].Text);
            numPetCrit.Value = Int32.Parse(lstPets.Items[index].SubItems[5].Text);
            numPetSpeed.Value = Int32.Parse(lstPets.Items[index].SubItems[6].Text);

            numPetLevel.Value = Int32.Parse(lstPets.Items[index].SubItems[0].Text);

            txtPetNickname.Text = lstPets.Items[index].SubItems[1].Text;

            cbPetType.Text = (Sog_Pets)playerObject.Pets[index].Type1 + "";
        }

        private void BtnDeleteSelectedItem_Click(object sender, EventArgs e)
        {
            int i = lstInventory.FocusedItem.Index;
            lstInventory.Items.RemoveAt(i);
        }

        private void NumPetStat_ValueChanged(object sender, EventArgs e)
        {
            if (lstPets.FocusedItem != null)
            {
                lstPets.Items[lstPets.FocusedItem.Index].SubItems[2].Text = numPetHP.Value.ToString();

                if (((Control)sender) == numPetHP)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[2].Text = numPetHP.Value.ToString();
                }
                else if (((Control)sender) == numPetEnergy)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[3].Text = numPetEnergy.Value.ToString();
                }
                else if (((Control)sender) == numPetDamage)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[4].Text = numPetDamage.Value.ToString();
                }
                else if (((Control)sender) == numPetCrit)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[5].Text = numPetCrit.Value.ToString();
                }
                else if (((Control)sender) == numPetSpeed)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[6].Text = numPetSpeed.Value.ToString();
                }
                else if (((Control)sender) == numPetLevel)
                {
                    lstPets.Items[lstPets.FocusedItem.Index].SubItems[0].Text = numPetLevel.Value.ToString();
                }
            }
        }

        private void TxtPetNickname_TextChanged(object sender, EventArgs e)
        {
            lstPets.Items[lstPets.FocusedItem.Index].SubItems[1].Text = txtPetNickname.Text;
        }

        private void JSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Include;
            serializer.Formatting = Formatting.Indented;

            string message = "Your savegame was saved on the Tool folder.";
            string caption = "Save complete";
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "/" + playerObject.UniquePlayerID + "_" + playerObject.Nickname + ".json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this.playerObject);
                MessageBox.Show(message, caption, buttons);
            }
        }

        //source: https://www.programmersought.com/article/973286506/
        private void TabControl1_DrawItem(object sender, DrawItemEventArgs e)  //Skills Menu, makes the Tabs go sidewards
        {
            SolidBrush _Brush = new SolidBrush(Color.Black); //monochrome brush
            RectangleF _TabTextArea = (RectangleF)tabControl1.GetTabRect(e.Index); //Drawing area
            StringFormat _sf = new StringFormat(); //Package text layout format information
            _sf.LineAlignment = StringAlignment.Center;
            _sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(tabControl1.Controls[e.Index].Text, SystemInformation.MenuFont, _Brush, _TabTextArea, _sf);
        }

        private void PictureBox3_Click(object sender, EventArgs e) //Cardano Icon in the top corner
        {
            FrmAbout frmAbout = new FrmAbout();
            frmAbout.ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e) { }
    }
}