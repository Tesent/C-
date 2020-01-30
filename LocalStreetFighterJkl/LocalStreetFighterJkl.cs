using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author Riku Laitinen
/// @version 21.4.2018
/// <summary>
/// Street Fighter Jyväskylä
/// </summary>


public class LocalStreetFighterJkl : PhysicsGame
{
    /// <summary>
    /// Alustetaan nopeuksia ja kartan koko
    /// </summary>
    public const double nopeus = 200.0;
    public const double hyppyNopeus = 750.0;
    public const int mapinKoko = 35;
    public const double AMMUKSEN_NOPEUS = 1000;
    private int pelaaja_num = 1;

    /// <summary>
    /// Kutsutaan pelattavat hahmot
    /// </summary>

    private PlatformCharacter pelaaja1;
    private PlatformCharacter pelaaja2;

    /// <summary>
    /// Ladataan tarvittavat kuvat
    /// </summary>


    private static readonly Image Mappi1_kuva = LoadImage("LEVELI1Test");
    private static readonly Image Pelaaja1_kuva = LoadImage("Pelaaja1");
    private static readonly Image Pelaaja2_kuva = LoadImage("Pelaaja2");
    private static readonly Image Pullo2_kuva = LoadImage("pullo2");
    private static readonly Image Puukko_kuva = LoadImage("puukko");

    /// <summary>
    /// Luodaan alkuvalikko
    /// </summary>


    private List<Label> valikonKohdat;

    /// <summary>
    /// Luo valikon ensimäistä kertaa
    /// </summary>
    public override void Begin()
    {
        Valikko();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Valikko()
    {
        ClearAll();

        valikonKohdat = new List<Label>();

        Label kohta1 = new Label("Aloita uusi peli");
        kohta1.Position = new Vector(0, 40);
        valikonKohdat.Add(kohta1);

        Label kohta2 = new Label("Ohjeet");
        kohta2.Position = new Vector(0, 0);
        valikonKohdat.Add(kohta2);

        Label kohta3 = new Label("Lopeta peli");
        kohta3.Position = new Vector(0, -40);
        valikonKohdat.Add(kohta3);

        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }
        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaPeli, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, Ohjeet, null);
        Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, Exit, null);
    }


    /// <summary>
    /// Tehdään aliohjelma joka aloittaa pelin
    /// </summary>
    /// 

    public void AloitaPeli()
    {
        ClearControls();
        ClearGameObjects();
        pelaaja_num = 1;

        IsPaused = false;

        Gravity = new Vector(0, -1000);
        Camera.ZoomFactor = 0.8;

        LuoMappi1();
        LisaaNappaimet();
    }


    /// <summary>
    /// Luodaan aliohjelma joka avaa uuden sivun jossa on ohjeet
    /// </summary>

    public void Ohjeet()
    {
        MessageWindow ohjeet1 = new MessageWindow(" Pelaaja1: Liiku = A, D, Hyppää = W , Heitä = Space" +
            Environment.NewLine + "Pelaaja2: Liiku = Vasen, Oikea, Hyppää = Ylös, Heitä = Enter");
        ohjeet1.Position = new Vector(0, 0);
        ohjeet1.Closed += delegate { Valikko(); };
        Add(ohjeet1);

        Keyboard.Listen(Key.Escape, ButtonState.Down, Valikko, null);
    }


    /// <summary>
    /// Luo kartan
    /// </summary>

    public void LuoMappi1()
    {
        TileMap Mappi1 = TileMap.FromLevelAsset("Mappi1");
        Mappi1.SetTileMethod('X', LuoTaso);
        Mappi1.SetTileMethod('P', LuoPelaaja);
        Mappi1.SetTileMethod('B', LuoPelaaja);
        Mappi1.SetTileMethod('C', LuoTappavaTaso);
        Mappi1.Execute(25, 25);

        Level.Background.Image = Mappi1_kuva;
    }


    /// <summary>
    /// Luo tason
    /// </summary>
    /// <param name="kohta"></param>
    /// <param name="leveys" Tason leveys></param>
    /// <param name="korkeus" Tason Korkeus></param>

    public void LuoTaso(Vector kohta, double leveys, double korkeus)
    {
        PhysicsObject Taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Taso.Position = kohta;
        Taso.Color = Color.Lime;
        Add(Taso);
    }


    /// <summary>
    /// Luo tason joka tuhoaa pelaajan siihen osuessaan
    /// </summary>
    /// <param name="kohta"></param>
    /// <param name="leveys" >Tappavan tason leveys</param>
    /// <param name="korkeus" >Tappavan tason korkeus ></param>

    public void LuoTappavaTaso(Vector kohta, double leveys, double korkeus)
    {
        PhysicsObject Taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Taso.Position = kohta;
        Taso.Color = Color.Gray;
        Taso.Tag = "tappavaTaso";

        Add(Taso);
    }

  

    /// <summary>
    /// Luo pelaajan
    /// </summary>
    /// <param name="kohta">Kohta mapissa</param>
    /// <param name="leveys">Pelaajan leveys</param>
    /// <param name="korkeus">Pelaajan korkeus</param>

    public void LuoPelaaja(Vector kohta, double leveys, double korkeus)
    {
        PlatformCharacter pelaaja_ = new PlatformCharacter(leveys, korkeus);
        pelaaja_.Position = kohta;
        pelaaja_.Mass = 3.0;
        pelaaja_.TurnsWhenWalking = false;


        if (pelaaja_num == 1)
        {
            pelaaja_num++;
            pelaaja1 = pelaaja_;
            pelaaja1.Image = Pelaaja1_kuva;
            AddCollisionHandler(pelaaja_, "pelaaja1Ammus", Pelaaja1Iskutloppuivat);
            AddCollisionHandler(pelaaja_, "tappavaTaso", Pelaaja1Iskutloppuivat);
            Add(pelaaja1);
            
        }
        else
        {
            pelaaja2 = pelaaja_;
            pelaaja2.Image = Pelaaja2_kuva;
            AddCollisionHandler(pelaaja_, "pelaaja2Ammus", Pelaaja2Iskutloppuivat);
            AddCollisionHandler(pelaaja_, "tappavaTaso", Pelaaja2Iskutloppuivat);
            Add(pelaaja2);
        }
    }


    /// <summary>
    /// Lisätään pelaajien näppäimet
    /// </summary>

    public void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, PauseValikko, "Lopeta peli");

        Keyboard.Listen(Key.A, ButtonState.Down, Liikuta, "Mene vasemalle", pelaaja1, -nopeus);
        Keyboard.Listen(Key.D, ButtonState.Down, Liikuta, "Mene Oikealle", pelaaja1, nopeus);
        Keyboard.Listen(Key.W, ButtonState.Pressed, Hyppaa, "Hyppää", pelaaja1, hyppyNopeus);
        Keyboard.Listen(Key.Space, ButtonState.Pressed, PulloAmmus, null, pelaaja1, "pelaaja2Ammus");


        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Mene vasemalle", pelaaja2, -nopeus);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Mene Oikealle", pelaaja2, nopeus);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Hyppää", pelaaja2, hyppyNopeus);
        Keyboard.Listen(Key.Enter, ButtonState.Pressed, PuukkoAmmus, null, pelaaja2, "pelaaja1Ammus");

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");
    }


    /// <summary>
    ///  Heittää pullon
    /// </summary>
    /// <param name="pullo" >Pelaaja1 heitettävä ammus </param>
    /// <param name="tunniste" >Tunniste ammukselle</param>
    /// 

    public void PulloAmmus(PhysicsObject ammukset, string tunniste)
    {
        PhysicsObject ammus = new PhysicsObject(Pullo2_kuva);
        Add(ammus);
        ammus.Position = ammukset.Position;
        ammus.Hit(new Vector(AMMUKSEN_NOPEUS, 0));
        ammus.Color = Color.Black;
        ammus.Tag = tunniste;
        ammus.Image = Pullo2_kuva;
        ammus.IgnoresGravity = true;
        ammus.MaximumLifetime = TimeSpan.FromSeconds(1.0);
    }


    /// <summary>
    /// Heittää puukon
    /// </summary>
    /// <param name="ammukset" >Pelaaja2 heitettävä ammus</param>
    /// <param name="tunniste" >Tunniste ammukselle</param>

    public void PuukkoAmmus(PhysicsObject ammukset, string tunniste)
    {
        PhysicsObject ammus = new PhysicsObject(Puukko_kuva);
        Add(ammus);
        ammus.Position = ammukset.Position;
        ammus.Hit(new Vector(-AMMUKSEN_NOPEUS, 0));
        ammus.Color = Color.Black;
        ammus.Tag = tunniste;
        ammus.IgnoresGravity = true;
        ammus.MaximumLifetime = TimeSpan.FromSeconds(1.0);
    }


    /// <summary>
    /// Pelaaja 1 häviää
    /// </summary>
    /// <param name="pelaaja2"></param>
    /// <param name="ammus"></param>

    public void Pelaaja1Iskutloppuivat(PhysicsObject pelaaja2, PhysicsObject ammus)
    {

        MessageWindow pelaaja1Voitti = new MessageWindow("Pelaaja 2 voitti!" + Environment.NewLine + " Voititte makkaraperunat!");
        pelaaja1Voitti.Position = new Vector(0, 250);

        Add(pelaaja1Voitti);

        pelaaja1.Destroy();

        PauseValikko();
    }


    /// <summary>
    /// Pelaaja 2 häviää
    /// </summary>
    /// <param name="pelaaja2"></param>
    /// <param name="ammus"></param>

    public void Pelaaja2Iskutloppuivat(PhysicsObject pelaaja2, PhysicsObject ammus)
    {

        Window pelaaja2Voitti = new MessageWindow("Pelaaja 1 voitti!" + Environment.NewLine + " Voititte makkaraperunat!");
        pelaaja2Voitti.Position = new Vector(0, 250);

        Add(pelaaja2Voitti);

        pelaaja2.Destroy();

        PauseValikko();
    }


    /// <summary>
    ///ESC Näppäimestä avaa valikon pelin aikana 
    /// </summary>

    public void PauseValikko()
    {
        IsPaused = true;

        MultiSelectWindow uusipeli = new MultiSelectWindow("LOCAL STREET FIGHTER JYVÄSKYLÄ",
       "uusi peli", "Päävalikko", "Lopeta");

        Add(uusipeli);

        uusipeli.AddItemHandler(0, UusiPeli);
        uusipeli.AddItemHandler(1, Valikko);
        uusipeli.AddItemHandler(2, Exit);
    }

    /// <summary>
    /// Pelaajan liikkuminen
    /// </summary>
    /// <param name="Pelaaja" >Pelattava hahmo</param>
    /// <param name="nopeus" >Asettaa pelaajan kävely nopeuden</param>

    public void Liikuta(PlatformCharacter pelaaja, double nopeus)
    {
        pelaaja.Walk(nopeus);
    }


    /// <summary>
    /// Pelaajan liikkuminen
    /// </summary>
    /// <param name="Pelaaja" >Pelattava hahmo</param>
    /// <param name="nopeus" >Asettaa pelaajan hyppy nopeuden</param>

    public void Hyppaa(PlatformCharacter pelaaja, double nopeus)
    {
        pelaaja.Jump(nopeus);
    }


    /// <summary>
    /// Aloittaa uuden pelin
    /// </summary>

    public void UusiPeli()
    {
        AloitaPeli();
    }


}
