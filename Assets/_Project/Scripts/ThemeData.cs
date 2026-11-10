
using System;
using System.Collections.Generic;

[Serializable]
public class ThemeData
{
    public List<bool> DogThemes = new List<bool>();
    public List<bool> BeeThemes = new List<bool>();
    public List<bool> BeehiveThemes = new List<bool>();

    public int SelectedDogTheme;
    public int SelectedBeeTheme;
    public int SelectedBeehiveTheme;
}
