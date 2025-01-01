using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public string playerName;
    public float movementSpeed;
    public float evolveRate;
    public int maxHealth;
    public int points;

    public bool PistolUnlocked;
    public float PistolDamage;
    public float PistolFireRate;
    public float PistolAccuracy;
    public float PistolMagSize;
    public float PistolReloadTime;

    public bool MGUnlocked;
    public float MGDamage;
    public float MGFireRate;
    public float MGAccuracy;
    public float MGMagSize;
    public float MGReloadTime;

    public bool ShotgunUnlocked;
    public float ShotgunDamage;
    public float ShotgunFireRate;
    public float ShotgunAccuracy;
    public float ShotgunMagSize;
    public float ShotgunReloadTime;

    public UserData(string playerName)
    {
        this.playerName = playerName;
        this.movementSpeed = 1.0f;
        this.evolveRate = 0.2f;
        this.maxHealth = 1;
        this.points = 0;

        this.PistolUnlocked = true;
        this.PistolDamage = 1.0f;
        this.PistolFireRate = 1.0f;
        this.PistolAccuracy = 1.0f;
        this.PistolMagSize = 1.0f;
        this.PistolReloadTime = 1.0f;

        this.MGUnlocked = false;
        this.MGDamage = 1.0f;
        this.MGFireRate = 1.0f;
        this.MGAccuracy = 1.0f;
        this.MGMagSize = 1.0f;
        this.MGReloadTime = 1.0f;

        this.ShotgunUnlocked = false;
        this.ShotgunDamage = 1.0f;
        this.ShotgunFireRate = 1.0f;
        this.ShotgunAccuracy = 1.0f;
        this.ShotgunMagSize = 1.0f;
        this.ShotgunReloadTime = 1.0f;
    }
    public UserData(string playerName, float movementSpeed, float evolveRate, int maxHealth, int points, bool pistolUnlocked, float pistolDamage, float pistolFireRate, float pistolAccuracy, float pistolMagSize, float pistolReloadTime, bool mGUnlocked, float dMGamage, float mGFireRate, float mGAccuracy, float mGMagSize, float mGReloadTime, bool shotgunUnlocked, float shotgunDamage, float shotgunFireRate, float shotgunAccuracy, float shotgunMagSize, float shotgunReloadTime)
    {
        this.playerName = playerName;
        this.movementSpeed = movementSpeed;
        this.evolveRate = evolveRate;
        this.maxHealth = maxHealth;
        this.points = points;

        this.PistolUnlocked = pistolUnlocked;
        this.PistolDamage = pistolDamage;
        this.PistolFireRate = pistolFireRate;
        this.PistolAccuracy = pistolAccuracy;
        this.PistolMagSize = pistolMagSize;
        this.PistolReloadTime = pistolReloadTime;

        this.MGUnlocked = mGUnlocked;
        this.MGDamage = dMGamage;
        this.MGFireRate = mGFireRate;
        this.MGAccuracy = mGAccuracy;
        this.MGMagSize = mGMagSize;
        this.MGReloadTime = mGReloadTime;

        this.ShotgunUnlocked = shotgunUnlocked;
        this.ShotgunDamage = shotgunDamage;
        this.ShotgunFireRate = shotgunFireRate;
        this.ShotgunAccuracy = shotgunAccuracy;
        this.ShotgunMagSize = shotgunMagSize;
        this.ShotgunReloadTime = shotgunReloadTime;
    }

    public string getPlayerName()
    {
        return playerName;
    }
    public float getMovementSpeed()
    {
        return movementSpeed;
    }
    public float getEvolveRate()
    {
        return evolveRate;
    }
    public int getMaxHealth()
    {
        return maxHealth;
    }
    public int getPoints()
    {
        return points;
    }

    public bool isPistolUnlocked()
    {
        return PistolUnlocked;
    }
    public float getPistolDamage()
    {
        return PistolDamage;
    }
    public float getPistolFireRate()
    {
        return PistolFireRate;
    }
    public float getPistolAccuracy()
    {
        return PistolAccuracy;
    }
    public float getPistolMag()
    {
        return PistolMagSize;
    }
    public float getPistolReload()
    {
        return PistolReloadTime;
    }

    public bool isMGUnlocked()
    {
        return MGUnlocked;
    }
    public float getMGDamage()
    {
        return MGDamage;
    }
    public float getMGFireRate()
    {
        return MGFireRate;
    }
    public float getMGAccuracy()
    {
        return MGAccuracy;
    }
    public float getMGMag()
    {
        return MGMagSize;
    }
    public float getMGReload()
    {
        return MGReloadTime;
    }

    public bool isShotgunUnlocked()
    {
        return ShotgunUnlocked;
    }
    public float getShotgunDamage()
    {
        return ShotgunDamage;
    }
    public float getShotgunFireRate()
    {
        return ShotgunFireRate;
    }
    public float getShotgunAccuracy()
    {
        return ShotgunAccuracy;
    }
    public float getShotgunMag()
    {
        return ShotgunMagSize;
    }
    public float getShotgunReload()
    {
        return ShotgunReloadTime;
    }

    public void setPlayerName(string n)
    {
        this.playerName = n;
    }
    public void setMovementSpeed(float f)
    {
        this.movementSpeed = f;
    }
    public void setEvolveRate(float f)
    {
        this.evolveRate = f;
    }
    public void setMaxHealth(int i)
    {
        this.maxHealth = i;
    }
    public void setPoints(int i)
    {
        this.points = i;
        
    }

    public void unlockPistol()
    {
        this.PistolUnlocked = true;
    }
    public void setPistolDamage(float f)
    {
        this.PistolDamage = f;
    }
    public void setPistolFireRate(float f)
    {
        this.PistolFireRate = f;
    }
    public void setPistolAccuracy(float f)
    {
        this.PistolAccuracy = f;
    }
    public void setPistolMag(float f)
    {
        this.PistolMagSize = f;
    }
    public void setPistolReload(float f)
    {
        this.PistolReloadTime = f;
    }

    public void unlockMG()
    {
        this.MGUnlocked = true;
    }
    public void setMGDamage(float f)
    {
        this.MGDamage = f;
    }
    public void setMGFireRate(float f)
    {
        this.MGFireRate = f;
    }
    public void setMGAccuracy(float f)
    {
        this.MGAccuracy = f;
    }
    public void setMGMag(float f)
    {
        this.MGMagSize = f;
    }
    public void setMGReload(float f)
    {
        this.MGReloadTime = f;
    }

    public void unlockShotgun()
    {
        this.ShotgunUnlocked = true;
    }
    public void setShotgunDamage(float f)
    {
        this.ShotgunDamage = f;
    }
    public void setShotgunFireRate(float f)
    {
        this.ShotgunFireRate = f;
    }
    public void setShotgunAccuracy(float f)
    {
        this.ShotgunAccuracy = f;
    }
    public void setShotgunMag(float f)
    {
        this.ShotgunMagSize = f;
    }
    public void setShotgunReload(float f)
    {
        this.ShotgunReloadTime = f;
    }

    public string toString()
    {
        string str = "Name: " + playerName;
        str += ", Player Speed: " + movementSpeed;
        str += ", Max Health: " + maxHealth;
        str += ", Evolve Rate: " + evolveRate;
        str += ", Points: " + points;

        str += ", Pistol Unlocked: " + PistolUnlocked;
        str += ", Pistol Accuracy: " + PistolAccuracy;
        str += ", Pistol FireRate: " + PistolFireRate;
        str += ", Pistol Damage: " + PistolDamage;
        str += ", Pistol Mag Size: " + PistolMagSize;

        str += ", Machine Gun Unlocked: " + MGUnlocked;
        str += ", Machine Gun Accuracy: " + MGAccuracy;
        str += ", Machine Gun FireRate: " + MGFireRate;
        str += ", Machine Gun Damage: " + MGDamage;
        str += ", Machine Gun Mag Size: " + MGMagSize;

        str += ", Shotgun Unlocked: " + ShotgunUnlocked;
        str += ", Shotgun Accuracy: " + ShotgunAccuracy;
        str += ", Shotgun FireRate: " + ShotgunFireRate;
        str += ", Shotgun Damage: " + ShotgunDamage;
        str += ", Shotgun Mag Size: " + ShotgunMagSize;


        return str;
    }
}
