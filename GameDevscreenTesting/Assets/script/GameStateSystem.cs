using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStateSystem : MonoBehaviour
{
   
    public Transform[] SetObjectsForSpawn;
    public Text TotalScoreDisplay;
    public Text TotalWinDisplay;
    
    [HideInInspector]
    public int TotalScore;
    [HideInInspector]
    public int CountWin;
   
    public List<Transform> SpawnsTransform = new List<Transform>();
    public List<Sprite> AvatarSprite_Load = new List<Sprite>();
    public List<Sprite> AvatarSpriteBg_Load = new List<Sprite>();

    [SerializeField]
    RandomStatusAllRange StatusSetRange;
    [SerializeField]
    SetMachanicGameClear SetMachanicGameClear;

    void Awake()
    {
        //load avatar and load transform.
        for (int looploadsprite = 1; looploadsprite <= 80; looploadsprite++)
        {
            var sprite = Resources.Load<Sprite>("sprites/"+ looploadsprite);
            AvatarSprite_Load.Add(sprite);
        }
        var spritemulti = Resources.LoadAll<Sprite>("sprites/all");
   
       for (int looploadspritemul = 1; looploadspritemul < 6; looploadspritemul+=2) {
            AvatarSpriteBg_Load.Add(spritemulti[looploadspritemul]);
        }
        if (SetMachanicGameClear.SetEnemySpawns.NumberLimit<=SetMachanicGameClear.SetWinsCountForClearGame) {
            SetMachanicGameClear.SetWinsCountForClearGame = SetMachanicGameClear.SetEnemySpawns.NumberLimit;
        }
        GameObject[] SpawnsGameobject = GameObject.FindGameObjectsWithTag("TransformSpawn_Pos");
        for (int loopaddspawn = 0;loopaddspawn < SpawnsGameobject.Length;loopaddspawn++) {
            SpawnsTransform.Add(SpawnsGameobject[loopaddspawn].transform);
        }
      //  QualitySettings.vSyncCount = 0;
      //  Application.targetFrameRate = 60;
       
    }
    void Start()
    {
        if (AvatarSprite_Load.Count >= 80)
        {
            RandomEnemyAndHero();
        }
    }
    //Random Enemy and hero when start game.
    public void RandomEnemyAndHero() {
        for (int loopspawnenemy=0;loopspawnenemy< SetMachanicGameClear.SetEnemySpawns.NumberLimit; loopspawnenemy++) {
            int Randoms_Transform = Random.Range(0,SpawnsTransform.Count-1);
            Vector3 FixPosRandom = new Vector3(SpawnsTransform[Randoms_Transform].position.x+Random.Range(-0.3f,0.3f), SpawnsTransform[Randoms_Transform].position.y + Random.Range(-0.3f, 0.3f), SpawnsTransform[Randoms_Transform].position.z);
            GameObject SpawnEnemy = Instantiate(SetObjectsForSpawn[1].gameObject, FixPosRandom, SpawnsTransform[Randoms_Transform].rotation);
            SpawnsTransform.RemoveAt(Randoms_Transform);

            Enemy_system SetValueEnemy = SpawnEnemy.GetComponent<Enemy_system>();
            SpawnEnemy.name = SpawnEnemy.name+loopspawnenemy;
            int randomsprites = Random.Range(StatusSetRange.SetEnemy.Sprite.SetMin, StatusSetRange.SetEnemy.Sprite.SetMax);
            SpawnEnemy.GetComponent<SpriteRenderer>().sprite = AvatarSprite_Load[randomsprites];
            SetValueEnemy.EnemySprite = AvatarSprite_Load[randomsprites];
            SetValueEnemy.Speed = float.Parse((Random.Range(StatusSetRange.SetEnemy.Speed.SetMin, StatusSetRange.SetEnemy.Speed.SetMax)/10.0f).ToString("F2"));
            SetValueEnemy.EnemySword = Random.Range(StatusSetRange.SetEnemy.Sword.SetMin, StatusSetRange.SetEnemy.Sword.SetMax);
            SetValueEnemy.EnemyShield = Random.Range(StatusSetRange.SetEnemy.Shield.SetMin, StatusSetRange.SetEnemy.Shield.SetMax);
            SetValueEnemy.EnemyHeart = Random.Range(StatusSetRange.SetEnemy.Heart.SetMin, StatusSetRange.SetEnemy.Heart.SetMax);
            SetValueEnemy.EnemyType = Random.Range(StatusSetRange.SetEnemy.Type.SetMin, StatusSetRange.SetEnemy.Type.SetMax);
            
            GameObject.Find(SpawnEnemy.name + "/type").GetComponent<SpriteRenderer>().sprite = AvatarSpriteBg_Load[SetValueEnemy.EnemyType];
            GameObject.Find(SpawnEnemy.name + "/type/Canvas/display/sword").GetComponent<Text>().text = "Sword : " + SetValueEnemy.EnemySword.ToString();
            GameObject.Find(SpawnEnemy.name + "/type/Canvas/display/shield").GetComponent<Text>().text = "Shield : " + SetValueEnemy.EnemyShield.ToString();
            GameObject.Find(SpawnEnemy.name + "/type/Canvas/display/heart").GetComponent<Text>().text = "Heart : " + SetValueEnemy.EnemyHeart.ToString();
            GameObject.Find(SpawnEnemy.name + "/type/Canvas/display/type").GetComponent<Text>().text = "Type : " + CheckTextType(SetValueEnemy.EnemyType);
            GameObject.Find(SpawnEnemy.name + "/type/Canvas/display/colordis").GetComponent<Image>().color = CheckColorType(SetValueEnemy.EnemyType);
        }

        for (int loopspawnavatar = 0; loopspawnavatar < SetMachanicGameClear.SetHeroSpawns.NumberLimit; loopspawnavatar++) {
            int Randoms_Transform = Random.Range(0, SpawnsTransform.Count-1);
            Vector3 FixPosRandom = new Vector3(SpawnsTransform[Randoms_Transform].position.x + Random.Range(-0.3f, 0.3f), SpawnsTransform[Randoms_Transform].position.y + Random.Range(-0.3f, 0.3f), SpawnsTransform[Randoms_Transform].position.z);
            GameObject SpawnHero = Instantiate(SetObjectsForSpawn[0].gameObject, FixPosRandom, SpawnsTransform[Randoms_Transform].rotation);
            SpawnsTransform.RemoveAt(Randoms_Transform);

            HeroSet SetValueHero = SpawnHero.GetComponent<HeroSet>();
            SpawnHero.name = SpawnHero.name + loopspawnavatar;
            int randomsprites = Random.Range(StatusSetRange.SetAvatar.Sprite.SetMin, StatusSetRange.SetAvatar.Sprite.SetMax);
           
            SpawnHero.GetComponent<SpriteRenderer>().sprite = AvatarSprite_Load[randomsprites];

            SetValueHero.AvatarSprite = AvatarSprite_Load[randomsprites];
            SetValueHero.Speed = float.Parse((Random.Range(StatusSetRange.SetAvatar.Speed.SetMin, StatusSetRange.SetAvatar.Speed.SetMax)/10.0f).ToString("F2"));
            SetValueHero.AvatarSword = Random.Range(StatusSetRange.SetAvatar.Sword.SetMin, StatusSetRange.SetAvatar.Sword.SetMax);
            SetValueHero.AvatarShield = Random.Range(StatusSetRange.SetAvatar.Shield.SetMin, StatusSetRange.SetAvatar.Shield.SetMax);
            SetValueHero.AvatarHeart = Random.Range(StatusSetRange.SetAvatar.Heart.SetMin, StatusSetRange.SetAvatar.Heart.SetMax);
            SetValueHero.AvatarType = Random.Range(StatusSetRange.SetAvatar.Type.SetMin, StatusSetRange.SetAvatar.Type.SetMax);

            GameObject.Find(SpawnHero.name + "/type").GetComponent<SpriteRenderer>().sprite = AvatarSpriteBg_Load[SetValueHero.AvatarType];
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/sword").GetComponent<Text>().text = "Sword : " + SetValueHero.AvatarSword.ToString();
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/shield").GetComponent<Text>().text = "Shield : " + SetValueHero.AvatarShield.ToString();
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/heart").GetComponent<Text>().text = "Heart : " + SetValueHero.AvatarHeart.ToString();
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/speed").GetComponent<Text>().text = "Speed : " + SetValueHero.Speed.ToString();
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/type").GetComponent<Text>().text = "Type : " + CheckTextType(SetValueHero.AvatarType);
            GameObject.Find(SpawnHero.name + "/type/Canvas/display/colordis").GetComponent<Image>().color = CheckColorType(SetValueHero.AvatarType);
        }
    }
    //Check colot with type number to display text ui.
    private string CheckTextType(int type)
    {
        string setcolortext = "";
        if (type == 0)
        {
            setcolortext = "Red";
        }
        if (type == 1)
        {
            setcolortext = "Green";
        }
        if (type == 2)
        {
            setcolortext = "Blue";
        }
        return setcolortext;
    }
    //Check colot with type number to display color ui.
    private Color CheckColorType(int type)
    {
        Color setcolor = Color.white;
        if (type == 0)
        {
            setcolor = Color.red;
        }
        if (type == 1)
        {
            setcolor = Color.green;
        }
        if (type == 2)
        {
            setcolor = Color.blue;
        }
        return setcolor;
    }
    //ฆcore add total and display to ui.
    public int ScoreAdd(int scoreplus) {
        if (scoreplus>0) {
            TotalScore += scoreplus;
            CountWin++;
            TotalWinDisplay.text = "Win Battle : "+ CountWin + " / 10";
            TotalScoreDisplay.text = "Score : " + TotalScore;
            if (CountWin >= SetMachanicGameClear.SetWinsCountForClearGame)
            {
                GameObject.Find("Avatar").GetComponent<Avatar_system>().GameFinish();            
            }
        }
        return TotalScore;
    }
    //restart game.
    public void ResetGame()
    {
        GameObject[] GameObjectDestroy = GameObject.FindGameObjectsWithTag("Enemy");
        for (int loopdelete=0;loopdelete< GameObjectDestroy.Length;loopdelete++) {
            GameObjectDestroy[loopdelete].name = "DeleteEmermy";
            Destroy(GameObjectDestroy[loopdelete]);
        }
        GameObjectDestroy = GameObject.FindGameObjectsWithTag("Hero");
        for (int loopdelete = 0; loopdelete < GameObjectDestroy.Length; loopdelete++)
        {
            GameObjectDestroy[loopdelete].name = "DeleteHero";
            Destroy(GameObjectDestroy[loopdelete]);
        }
        SpawnsTransform = new List<Transform>();
        GameObject[] SpawnsGameobject = GameObject.FindGameObjectsWithTag("TransformSpawn_Pos");
        for (int loopaddspawn = 0; loopaddspawn < SpawnsGameobject.Length; loopaddspawn++)
        {
            SpawnsTransform.Add(SpawnsGameobject[loopaddspawn].transform);
        }
        RandomEnemyAndHero();
    }
}

//------------- data structure in setting game system. ---------------//
[System.Serializable]
public class SetMachanicGameClear
{
    [SerializeField]
    public HeroSpawns SetHeroSpawns;
    [SerializeField]
    public HeroSpawns SetEnemySpawns;
    public int SetWinsCountForClearGame;
}
[System.Serializable]
public class HeroSpawns
{
    public int NumberLimit;
}
[System.Serializable]
public class EnemySpawns
{
    public int NumberLimit;
}


[System.Serializable]
public class RandomStatusAllRange
{
    // public int SetMinAvatar;
    // public int SetMax;
    [SerializeField]
    public SetValue SetAvatar;
    [SerializeField]
    public SetValue SetEnemy;

   
}

[System.Serializable]
public class SetValue
{
    [SerializeField]
    public SetRange Sprite;
    [SerializeField]
    public SetRange Speed;
    [SerializeField]
    public SetRange Sword;
    [SerializeField]
    public SetRange Shield;
    [SerializeField]
    public SetRange Heart;
    [SerializeField]
    public SetRange Type;
}

[System.Serializable]
public class SetRange
{
    public int SetMin;
    public int SetMax;
}
//-----------------------------//