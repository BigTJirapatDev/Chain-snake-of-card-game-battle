using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Avatar_system : MonoBehaviour
{
    public List<GameObject> Avatar_Container = new List<GameObject>();
    public List<int> Avatar_ListSave = new List<int>();
    int CountTime;
    public Sprite AvatarSprite;
    public int CountTimeLimit;
    public int SetValueAxisRotation;
    public float Speed;
    public int AvatarSword;
    public int AvatarShield;
    public int AvatarHeart;
    public int AvatarType;

    private bool IsBattleEnemy;
    private bool IsFollow;
    private bool IsClashWall;
    private bool IsGameOver;
    private bool IsGameFinish;
    private bool IsWinBattle;
    private bool IsPauseGame;
    private GameStateSystem SetDisplayScores;

    bool IsAttackEnemy;
    bool IsEnemyAttack;
    private IEnumerator CombatResolve;
    private IEnumerator UpdateTransformFollow;
    private Vector2[] TempPosotionAvatar;
    private int AvatarSelectHeros = 0;
    private GetDataHerosInAvatar SaveStatus = new GetDataHerosInAvatar();
    public GameObject EnemyTargetBattle;
    private int multiply = 1;
    public Text DisplayResualt;
    public Text DisplayCharactorUse;
    public Text SpeedUpDisplay;
    public GameObject SetStateOver;
    public GameObject ResetGameButton;
    public GameObject StateFinish;
    public GameObject PauseImage;
    public float SpeedAddUp;

    float SpeedForWin = 1.0f;
    string DisplayValue = "";
    private Vector3 TempPosStart;



    void Start()
    {
        TempPosStart = transform.position;
        SetDisplayScores = GameObject.Find("GameStateSystem").GetComponent<GameStateSystem>();

        AvatarSprite = SetDisplayScores.AvatarSprite_Load[Random.Range(0, SetDisplayScores.AvatarSprite_Load.Count-1)];
        GetComponent<SpriteRenderer>().sprite = AvatarSprite;
        
        Speed = float.Parse((Random.Range(5.0f, 10.0f)/10.0f).ToString("F2"));
        AvatarSword = Random.Range(3,10);
        AvatarShield = Random.Range(3,10);
        AvatarHeart = Random.Range(3,10);
        AvatarType = Random.Range(0,3);
        GameObject.Find(this.name + "/type").GetComponent<SpriteRenderer>().sprite = SetDisplayScores.AvatarSpriteBg_Load[AvatarType];
        SaveStatus.Avatar_Sprite.Add(AvatarSprite);
        SaveStatus.Speed.Add(Speed);
        SaveStatus.AvatarSword.Add(AvatarSword);
        SaveStatus.AvatarShield.Add(AvatarShield);
        SaveStatus.AvatarHeart.Add(AvatarHeart);
        SaveStatus.AvatarType.Add(AvatarType);
        Avatar_Container.Add(this.gameObject);
        EnemyTargetBattle = this.gameObject;
        SetDisplayStatus();
    }
    private void AppendHeroToAvatar(GameObject HeroAddToAvatar) { // When touch hero to add Hero to team.

        GameObject.Find(HeroAddToAvatar.name + "/type/Canvas/display").SetActive(false);
        SaveDataStatustoAvatar(HeroAddToAvatar.GetComponent<HeroSet>());
        GameObject Hero_Add = Instantiate(HeroAddToAvatar) as GameObject;
        Hero_Add.name = HeroAddToAvatar.name;
        HeroSet Remove_new = Hero_Add.GetComponent<HeroSet>();
        Rigidbody2D Remove_Rigid = Hero_Add.GetComponent<Rigidbody2D>();
        BoxCollider2D Remove_Collider = Hero_Add.GetComponent<BoxCollider2D>();

        Destroy(Remove_new);
        Destroy(Remove_Rigid);
        Destroy(Remove_Collider);
        if (!Avatar_Container.Contains(Hero_Add))
        {
            Avatar_Container.Add(Hero_Add);
        }
        Destroy(HeroAddToAvatar);
    }
    public void TurnAvatarLeftOrRight(int RotateAxis)
    {
        this.transform.eulerAngles += this.transform.forward * RotateAxis * SetValueAxisRotation;
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsGameOver) {
            if (!IsGameFinish)
            {         
                if (!IsBattleEnemy)
                {
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        TurnAvatarLeftOrRight(-1);
                    }
                    if (Input.GetKeyDown(KeyCode.D))
                    {
                        TurnAvatarLeftOrRight(1);
                    }
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        SwitchAvatar(-1);
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SwitchAvatar(1);
                    }
                    if (Input.GetKeyDown(KeyCode.P)&&!IsPauseGame)
                    {
                        Time.timeScale = 0;
                        IsPauseGame = true;
                        PauseImage.SetActive(true);
                    }
                    else if (Input.GetKeyDown(KeyCode.P) &&IsPauseGame)
                    {
                       Time.timeScale = 1;
                        IsPauseGame = false;
                        PauseImage.SetActive(false);
                    }
                    Movingforward();
                    if (Avatar_Container.Count > 1)
                    {
                        if (IsFollow == false)
                        {
                            UpdateTransformFollow = UpdateTempTransform(0.6f/SpeedForWin);
                            StartCoroutine(UpdateTransformFollow);
                        }
                    }
                }
                else
                {
                    if (IsAttackEnemy || IsEnemyAttack)
                    {
                        CombatResolve = ProcessCombatResolve(0.5f);
                        StartCoroutine(CombatResolve);
                    }
                    if (!IsAttackEnemy && !IsEnemyAttack)
                    {
                        IsAttackEnemy = true;
                        IsEnemyAttack = true;
                    }
                }
            }
            else {

            }
        }
    }

    private void Movingforward() { //move forward avatar auto direct.
        this.transform.position += (transform.up * Time.deltaTime) * (Speed * SpeedForWin);
    }

    private IEnumerator UpdateTempTransform(float waitTime) //move new hero in team follow avatar.
    {
        IsFollow = true;
        for (int loopupdate = Avatar_Container.Count - 1; loopupdate > 0; loopupdate--) {
            if (loopupdate > 0) {

                Avatar_Container[loopupdate].transform.position = Avatar_Container[loopupdate - 1].transform.position;
                Avatar_Container[loopupdate].transform.eulerAngles = Avatar_Container[loopupdate - 1].transform.eulerAngles;
            }
        }
        Avatar_Container[0].transform.position = transform.position;
        yield return new WaitForSeconds(waitTime);
        IsFollow = false;
    }

    private void SaveDataStatustoAvatar(HeroSet GetStatus) //Add save status all heros to avatar database.
    {
        SaveStatus.Avatar_Sprite.Add(GetStatus.AvatarSprite);
        SaveStatus.Speed.Add(GetStatus.Speed);
        SaveStatus.AvatarSword.Add(GetStatus.AvatarSword);
        SaveStatus.AvatarShield.Add(GetStatus.AvatarShield);
        SaveStatus.AvatarHeart.Add(GetStatus.AvatarHeart);
        SaveStatus.AvatarType.Add(GetStatus.AvatarType);
    }

    private IEnumerator ProcessCombatResolve(float waitTime) //loop process combat until combat battle end.
    {
        EnemyTargetBattle.transform.eulerAngles = new Vector3(0, 0, this.gameObject.transform.eulerAngles.z + 180);
        transform.position -= transform.up * 0.001f;
        yield return new WaitForSeconds(waitTime);
        transform.position += transform.up * 0.001f;
        if (EnemyTargetBattle != this.gameObject) {
            if (EnemyTargetBattle.GetComponent<Enemy_system>().EnemyType == AvatarType) {
                multiply = 1 * 2;
            }
            else {
                multiply = 1;
            }
            int ResultCheck = 0;
            if (IsAttackEnemy) {
                IsAttackEnemy = false;

                ResultCheck = BattleCalculate(AvatarSword, EnemyTargetBattle.GetComponent<Enemy_system>().EnemyShield­, EnemyTargetBattle.GetComponent<Enemy_system>().EnemyHeart, true);
                if (ResultCheck <= 0)
                {
                    //Debug.Log("Avatar Win!");
                    IsBattleEnemy = false;
                    IsAttackEnemy = false;
                    IsEnemyAttack = false;
                    EnemyTargetBattle.GetComponent<Enemy_system>().DestroyObject();
                    if (!IsWinBattle) {

                        ResultCheck = 0;
                        EnemyTargetBattle = this.gameObject;
                        DisplayValue += "\n  Avatar Win! Enemy Lose!";
                        DisplayResualt.text = DisplayValue;
                        DisplayValue = "";
                        int totalwinscores = 0;
                        for (int LoopAddScore = 0; LoopAddScore <= Avatar_Container.Count - 1; LoopAddScore++) {
                            totalwinscores += SaveStatus.AvatarHeart[LoopAddScore];
                        }
                        SetDisplayScores.ScoreAdd(totalwinscores);
                        
                        SpeedForWin += SpeedAddUp;
                        IsWinBattle = true;
                        SetDisplayStatus();
                    }
                }
                else {
                   // Debug.Log("Enemy Not Fail!");
                    DisplayValue += "\n  Enemy Turn!";
                    DisplayResualt.text = DisplayValue;
                    EnemyTargetBattle.GetComponent<Enemy_system>().EnemyHeart -= ResultCheck;
                }
            }
            EnemyTargetBattle.transform.position -= transform.up * 0.001f;
            yield return new WaitForSeconds(waitTime);
            EnemyTargetBattle.transform.position += transform.up * 0.001f;
            ResultCheck = 0;
            if (IsEnemyAttack)
            {
                IsEnemyAttack = false;
                ResultCheck = BattleCalculate(EnemyTargetBattle.GetComponent<Enemy_system>().EnemyShield, AvatarSword, AvatarHeart, false);
                if (ResultCheck <= 0)
                {
                    //Debug.Log("Enemy Win!");
                    DisplayValue += "\n Enemy Win! Avatar Lose!";
                    DisplayResualt.text = DisplayValue;
                    DisplayValue = "";
                    AvatarDestroy(true);
                }
                else
                {
                    //Debug.Log("Avatar Turn!");
                    DisplayValue += "\n  Avatar Turn!";
                    DisplayResualt.text = DisplayValue;
                    AvatarHeart -= ResultCheck;
                    SaveStatus.AvatarHeart[AvatarSelectHeros] = AvatarHeart;
                    SetDisplayStatus();
                    ResultCheck = 0;
                }
            }
        }
    }

    private int BattleCalculate(int Attacker, int Defender, int DefenderHeart, bool IsAvatar) { //Formula battle calculate.
        int result = (Attacker * multiply) - Defender;
        if (result <= 0) {
            result = 1;
        }
        DisplayValue += DisplayResaultCheck(Attacker, Defender, result, DefenderHeart, IsAvatar);
        DefenderHeart -= result;

        return DefenderHeart;
    }

    private string DisplayResaultCheck(int Attacker, int Defender, int Resualt, int DefenderHeart, bool IsAvatar) { //print texts resault to UI.
        if (IsAvatar) {
            DisplayValue = "Avartar Attack Damage to Enemy = " + Attacker
                                   + "\n Enemy Sheld" + Defender
                                   + "\n  Damage to Enemy" + Resualt
                                   + "\n  Enemy Hearth" + DefenderHeart;
        }
        else {
            DisplayValue = "Enemy Attack Damage to Avatar" + Attacker
                                   + "\n  Avatar Sheld" + Defender
                                   + "\n  Damage toAvatar" + Resualt
                                   + "\n  Avatar Hearth" + DefenderHeart;
        }
        return DisplayValue;
    }

    private void AvatarDestroy(bool IsEnemyWin) { //Destroy heros in team when loss battle and clash wall.

        if (Avatar_Container.Count > 1) { //if heros in team more one.
            Destroy(Avatar_Container[Avatar_Container.Count - 1]);
            Avatar_Container.RemoveAt((Avatar_Container.Count - 1));

            SaveStatus.Avatar_Sprite.RemoveAt(AvatarSelectHeros);
            SaveStatus.Speed.RemoveAt(AvatarSelectHeros);
            SaveStatus.AvatarSword.RemoveAt(AvatarSelectHeros);
            SaveStatus.AvatarShield.RemoveAt(AvatarSelectHeros);
            SaveStatus.AvatarHeart.RemoveAt(AvatarSelectHeros);
            SaveStatus.AvatarType.RemoveAt(AvatarSelectHeros);
            if (!IsEnemyWin)
            {
                this.gameObject.transform.eulerAngles = new Vector3(0, 0, this.gameObject.transform.eulerAngles.z + 180);
            }
            GetComponent<SpriteRenderer>().sprite = SaveStatus.Avatar_Sprite[0];
            Speed = SaveStatus.Speed[0];
            AvatarSword = SaveStatus.AvatarSword[0];
            AvatarShield = SaveStatus.AvatarShield[0];
            AvatarHeart = SaveStatus.AvatarHeart[0];
            AvatarType = SaveStatus.AvatarType[0];
            AvatarSprite = SaveStatus.Avatar_Sprite[0];

            for (int loopsetavatar = 1; loopsetavatar < Avatar_Container.Count; loopsetavatar++) {
                Avatar_Container[loopsetavatar].GetComponent<SpriteRenderer>().sprite = SaveStatus.Avatar_Sprite[loopsetavatar];
            }
            AvatarSelectHeros = 0;
            SetDisplayStatus();
            IsClashWall = false;

        }
        else { //if hero has one.
            SetStateOver.GetComponentInChildren<Text>().text = "Game Over!";
            ResetGameButton.SetActive(true);
            SpriteRenderer[] SetClose = GetComponentsInChildren<SpriteRenderer>();
            for (int loopsprite = 0; loopsprite < SetClose.Length; loopsprite++)
            {
                SetClose[loopsprite].enabled = false;
            }
            SetStateOver.SetActive(true);
            IsGameOver = true;
        }
    }

    public void SwitchAvatar(int SwitchSelection) { // Switch heros form avatar database.
        if (AvatarSelectHeros >= 0&&Avatar_Container.Count>1) {
            AvatarSelectHeros += SwitchSelection;
            if (AvatarSelectHeros <= 0) {
                AvatarSelectHeros = 0;
            }
            if (AvatarSelectHeros >= Avatar_Container.Count - 1) {
                AvatarSelectHeros = Avatar_Container.Count - 1;
            }

            if (Avatar_Container.Count > 1)
            {
                Avatar_ListSave = new List<int>();
                for (int x = 0; x < Avatar_Container.Count; x++)
                {
                    Avatar_ListSave.Add(x);
                }

                Avatar_ListSave.RemoveAt(AvatarSelectHeros);
            }

            for (int loopgetlist = 0; loopgetlist < Avatar_ListSave.Count; loopgetlist++) {
                Avatar_Container[loopgetlist + 1].GetComponent<SpriteRenderer>().sprite = SaveStatus.Avatar_Sprite[(Avatar_ListSave[loopgetlist])];
                GameObject.Find(Avatar_Container[loopgetlist + 1].name + "/type").GetComponent<SpriteRenderer>().sprite = SetDisplayScores.AvatarSpriteBg_Load[(SaveStatus.AvatarType[(Avatar_ListSave[loopgetlist])])];
            }

            Avatar_Container[0].GetComponent<SpriteRenderer>().sprite = SaveStatus.Avatar_Sprite[AvatarSelectHeros];
            DisplayCharactorUse.text = AvatarSelectHeros.ToString();

            Speed = SaveStatus.Speed[AvatarSelectHeros];
            AvatarSword = SaveStatus.AvatarSword[AvatarSelectHeros];
            AvatarShield = SaveStatus.AvatarShield[AvatarSelectHeros];
            AvatarHeart = SaveStatus.AvatarHeart[AvatarSelectHeros];
            AvatarType = SaveStatus.AvatarType[AvatarSelectHeros];
            AvatarSprite = SaveStatus.Avatar_Sprite[AvatarSelectHeros];
            GameObject.Find(this.name + "/type").GetComponent<SpriteRenderer>().sprite = SetDisplayScores.AvatarSpriteBg_Load[AvatarType];

            SetDisplayStatus();
        }
    }
    public void GameFinish() { //When game finish.
        IsGameFinish = true;
        ResetGameButton.SetActive(true);
        StateFinish.SetActive(true);
        GameObject.Find(StateFinish.name + "/StateFinishDetail/Text2").GetComponent<Text>().text = "HeroGet: "+Avatar_Container.Count+"/10 Bonus score * "+(1+((float)Avatar_Container.Count/10.0f))+"";
        int totalscoreplusbonus = SetDisplayScores.TotalScore*(1+(Avatar_Container.Count/10));
        GameObject.Find(StateFinish.name + "/StateFinishDetail/Text1").GetComponent<Text>().text = "Total Scores : " + totalscoreplusbonus;
    }

    public void ResetGame() { //When restart play game again.
      
        IsGameOver = false;
        IsGameFinish = false;
        IsClashWall = false;
        IsBattleEnemy = false;
        SetDisplayScores.TotalScore = 0;
        SetDisplayScores.CountWin = 0;
        SetDisplayScores.TotalWinDisplay.text = "Win Battle : " + 0 + " / 10";
        SetDisplayScores.TotalScoreDisplay.text = "Score : " + 0;
        DisplayCharactorUse.text = "0";

        if (Avatar_Container.Count>1) {
            for (int loopRemove = Avatar_Container.Count-1; loopRemove >= 1; loopRemove--)
            {
                Avatar_Container.RemoveAt(loopRemove);
            }
        }
        SetDisplayScores.ResetGame();
        SpriteRenderer[] SetClose = GetComponentsInChildren<SpriteRenderer>();
        for (int loopsprite = 0; loopsprite < SetClose.Length; loopsprite++)
        {
            SetClose[loopsprite].enabled = true;
        }
        SetStateOver.SetActive(false);
        ResetGameButton.SetActive(false);
        StateFinish.SetActive(false);

        AvatarSprite = SetDisplayScores.AvatarSprite_Load[Random.Range(0, SetDisplayScores.AvatarSprite_Load.Count - 1)];
        GetComponent<SpriteRenderer>().sprite = AvatarSprite;
        Speed = float.Parse((Random.Range(5.0f, 10.0f) / 10.0f).ToString("F2"));
        AvatarSword = Random.Range(3, 10);
        AvatarShield = Random.Range(3, 10);
        AvatarHeart = Random.Range(3, 10);
        AvatarType = Random.Range(0, 3);
        GameObject.Find(this.name + "/type").GetComponent<SpriteRenderer>().sprite = SetDisplayScores.AvatarSpriteBg_Load[AvatarType];
        SaveStatus.Avatar_Sprite[0]= AvatarSprite;
        SaveStatus.Speed[0] = Speed;
        SaveStatus.AvatarSword[0] = AvatarSword;
        SaveStatus.AvatarShield[0] = AvatarShield;
        SaveStatus.AvatarHeart[0] = AvatarHeart;
        SaveStatus.AvatarType[0] = AvatarType;
        transform.position = TempPosStart;
        transform.eulerAngles = Vector3.zero;
        EnemyTargetBattle = this.gameObject;
        SpeedForWin = 1.0f;
        SetDisplayStatus();
    }

    private void SetDisplayStatus() { //Display Status to Avatr UI.
        GameObject.Find(this.name + "/type/Canvas/display/sword").GetComponent<Text>().text = "Sword : " + AvatarSword.ToString();
        GameObject.Find(this.name + "/type/Canvas/display/shield").GetComponent<Text>().text = "Shield : " + AvatarShield.ToString();
        GameObject.Find(this.name + "/type/Canvas/display/heart").GetComponent<Text>().text = "Heart : " + AvatarHeart.ToString();
        GameObject.Find(this.name + "/type/Canvas/display/speed").GetComponent<Text>().text = "Speed : " + (Speed*SpeedForWin).ToString("f2");
        GameObject.Find(this.name + "/type/Canvas/display/type").GetComponent<Text>().text = "Type : " + CheckTextType(AvatarType);
        GameObject.Find(this.name + "/type/Canvas/display/colordis").GetComponent<Image>().color = CheckColorType(AvatarType);
        SpeedUpDisplay.text = "Speed Up X "+SpeedForWin;
    }

    private string CheckTextType(int type) //Check types number in database to display text type.
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

    private Color CheckColorType(int type) //Check types number in database to color ui type to ui canvas.
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

    private void OnTriggerEnter2D(Collider2D collision) //logic tricker when this object collision other object by tag.
    {
        if (collision.tag == "Wall") {
            //Debug.Log("Wall!");
            if (!IsClashWall)
            {
                IsClashWall = true;
                AvatarDestroy(false);
            }
        }
        if (collision.tag == "Enemy") {
            // Debug.Log("Enemy!");
             IsWinBattle = false;
             IsBattleEnemy = true;
            EnemyTargetBattle = collision.gameObject;
        }
        if (collision.tag == "Hero") {
            //Debug.Log("Hero!");
            AppendHeroToAvatar(collision.gameObject);
        }
    }

    private class GetDataHerosInAvatar //structure hero in avatar database.
    {
        public List<Sprite> Avatar_Sprite = new List<Sprite>();
        public List<float> Speed = new List<float>();
        public List<int> AvatarSword = new List<int>();
        public List<int> AvatarShield = new List<int>();
        public List<int> AvatarHeart = new List<int>();
        public List<int> AvatarType = new List<int>();
    }
}
