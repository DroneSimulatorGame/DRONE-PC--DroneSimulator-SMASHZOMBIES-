using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    public static AudioContainer Instance;
    
    public AudioSource press_Play, close_window, open_window, store_window, settings_window, buy_gold, exchange_S, rotate_S, ticked_S, drone_audio, gun_audio, comman_audio, workshop_audio, 
    tower_audio, payload_audio, wall_audio, architec_audio, 
    missile_audio, main_music, launcherturn, rockethit, bombsound, minesound, missile_takeoff, zonewarning, basewarning, 
    zombiediesound, zombiehitsound, t1destroyed, t2destroyed, t3destroyed, t4destroyed, Dcommand, Darchitech, Dworkshop, 
    Dpayload, Dwall, returnerror, tornadosound, noXPerror, nointerneterror, commanderror, architecerror, windsound, steelreward, goldreward, starsoundeffect, bomber_beep, HoockS;


    //public bool IsAudioOn;

    public MainScript gunshoot;
    public MyJoystickNew2 dronesound;
    public MissileLauncher missilelauncher;

    private void Awake()
    {
        Instance = this;
    }

    public void SFXIsOn()
    {
            press_Play.mute = false;
            close_window.mute = false;
            open_window.mute = false;
            store_window.mute = false;
            buy_gold.mute = false;
            exchange_S.mute = false;
            rotate_S.mute = false;
            ticked_S.mute = false;
            settings_window.mute = false;
            drone_audio.mute = false;
            gun_audio.mute = false;
        missile_audio.mute = false;
        comman_audio.mute = false;
        workshop_audio.mute = false;
        tower_audio.mute = false;
        payload_audio.mute = false;
        wall_audio.mute = false;
        architec_audio.mute = false;
        gunshoot.audioFX_start.mute = false;
        gunshoot.audioFX_end.mute = false;
        gunshoot.audioFX_loop.mute = false;
        dronesound.droneSound.mute = false;
        missilelauncher.audioSource.mute = false;
        rockethit.mute = false;
        bombsound.mute = false;
        minesound.mute = false;
        missile_takeoff.mute = false;
        zonewarning.mute = false;
        basewarning.mute = false;
        zombiediesound.mute = false;
        zombiehitsound.mute = false;
        t1destroyed.mute = false;
        t2destroyed.mute = false;
        t3destroyed.mute = false;
        t4destroyed.mute = false;
        Dcommand.mute = false;
        Darchitech.mute = false;
        Dworkshop.mute = false;
        Dpayload.mute = false;
        Dwall.mute = false;
        returnerror.mute = false;
        tornadosound.mute = false;
        noXPerror.mute = false;
        nointerneterror.mute = false;
        commanderror.mute = false;
        architecerror.mute = false;
        windsound.mute = false;
        goldreward.mute = false;
        steelreward.mute = false;
        starsoundeffect.mute = false;
        bomber_beep.mute = false;
        HoockS.mute = false;

    }



    public void SFXIsOff()
    {
        press_Play.mute = true;
        close_window.mute = true;
        open_window.mute = true;
        store_window.mute = true;
        buy_gold.mute = true;
        exchange_S.mute = true;
        rotate_S.mute = true;
        ticked_S.mute = true;
        settings_window.mute = true;
        drone_audio.mute = true;
        gun_audio.mute = true;
        missile_audio.mute = true;
        comman_audio.mute = true;
        workshop_audio.mute = true;
        tower_audio.mute = true;
        payload_audio.mute = true;
        wall_audio.mute = true;
        architec_audio.mute = true;
        gunshoot.audioFX_start.mute = true;
        gunshoot.audioFX_end.mute = true;
        gunshoot.audioFX_loop.mute = true;
        dronesound.droneSound.mute = true;
        missilelauncher.audioSource.mute = true;
        rockethit.mute = true;
        bombsound.mute = true;
        minesound.mute = true;
        missile_takeoff.mute = true;
        zonewarning.mute = true;
        basewarning.mute = true;
        zombiediesound.mute = true;
        zombiehitsound.mute = true;
        t1destroyed.mute = true;
        t2destroyed.mute = true;
        t3destroyed.mute = true;
        t4destroyed.mute = true;
        Dcommand.mute = true;
        Darchitech.mute = true;
        Dworkshop.mute = true;
        Dpayload.mute = true;
        Dwall.mute = true;
        returnerror.mute = true;
        tornadosound.mute = true;
        noXPerror.mute = true;
        nointerneterror.mute = true;
        commanderror.mute = true;
        architecerror.mute = true;
        windsound.mute = true;
        goldreward.mute = true;
        steelreward.mute = true;
        starsoundeffect.mute = true;
        bomber_beep.mute = true;
        HoockS.mute = true;


    }

    //public void AudioON()
    //{
    //    main_music.mute = false;
    //}

    //public void AudioOFF()
    //{
    //    main_music.mute = true;
    //}

    public void HoockSS()
    {
        HoockS.Play();
    }



    public void pressPlay()
    {
            press_Play.Play();
    }

    public void closeWindow()
    {
        close_window.Play();
    }

    public void openWindow()
    {
        open_window.Play();
    }

    public void storeWindow()
    {
        store_window.Play();
    }

    public void settingsWindow()
    {
        settings_window.Play();
    }

    public void buyGold()
    {
        buy_gold.Play();
    }

    public void exchangeS()
    {
        exchange_S.Play();
    }


    public void rotateS()
    {
        rotate_S.Play();
    }

    public void tickedS()
    {
        ticked_S.Play();
    }

    public void commanaudio()
    {
        comman_audio.Play();
    }

    public void workshopaudio()
    {
        workshop_audio.Play();
    }

    public void toweraudio()
    {
        tower_audio.Play();
    }

    public void architecaudio()
    {
        architec_audio.Play();
    }

    public void payloadaudio()
    {
        payload_audio.Play();
    }

    public void wallaudio()
    {
        wall_audio.Play();
    }

    public void missileAudio()
    {
        missile_audio.Play();
    }


}
