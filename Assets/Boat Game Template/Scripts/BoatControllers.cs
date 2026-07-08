using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BoatControllers : MonoBehaviour {
    // 操控方式
    public enum ControlTypes {
        Keyboard, Accelerometer,TapControls
    };
    public ControlTypes controlType;
    // 船体模型
    public Transform graphic;
    // 模型初始位置
    Vector3 graphicsPosition;
    // 马达/螺旋桨
    public Transform[] rotors;
    // 螺旋桨转速
    public float rotorSpeed;
    // 水波参数
    public float waveHeight = 5.0f;
    public float waveFrequency = 2.0f;
    // 转向速度
    public float turnSpeed = 5.0f;
    // 最大转向角度
    public float maxTurnAngle = 30.0f;
    // 速度管理器
    public ScrollController speedManager;
    // 主控制器
    public GameController gameController;
    // 各类粒子特效
    public ParticleSystem speedParticle;
    public ParticleSystem[] floatparticles;
    public ParticleSystem startRipple;
    // 当前左右输入值
    float horizontalInput = 0.0f;
    // 船体动画机
    Animator myAnimator;
    // 是否已结束
    bool isOver = false;
    // 是否已开始
    bool isStarted = false;
    // 道具持续时间
    public float shieldDuration = 2.5f;
    public float gasDuration = 2.5f;
    public float magnetRange = 5.0f;
    public float magnetDuration = 10.0f;
    // 道具特效
    public GameObject shieldGraphic;
    public GameObject magnetGraphic;
    // 护盾是否生效
    bool isProtected = false;
    // 收集到的金币与宝石
    int coins = 0;
    int gems = 0;
    // 当前得分
    float score = 0.0f;
    // 磁铁计时
    float lastMagnetTime;
    // 默认高度
    float defaultPosition;
    // 船体音源
    AudioSource audioPlayer;
    // 各类音效
    public AudioClip coinSFX;
    public AudioClip shieldSFX;
    public AudioClip magnetSFX;
    public AudioClip gasSFX;
    public AudioClip crashedSFX;
    public AudioClip reviveSFX;
    public AudioClip shieldOffSFX;
    public AudioClip gemSFX;
    // UI 文本
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI gemText;
    public TextMeshProUGUI scoreText;
    private Vector3 detailMove;


    public bool isContinue=false;

    private JoyStickMove joyStickMove;
    private static int moveSpeed=5;

    public GameObject GameUI;
    // 初始化各项数值与引用
    void Start() {
        Time.timeScale = 1.0f;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        audioPlayer = transform.GetComponent<AudioSource>();

        if (coinText)
            coinText.text = "X " + coins.ToString();
        if (gemText)
            gemText.text = "X " + gems.ToString();
        if (scoreText)
        {
            scoreText.text = "�÷�:" + score.ToString("F0");
        }

        if (speedManager) {
            speedManager.enabled = false;
        }

        if (graphic)
        {
            graphicsPosition = graphic.position;
        }

        if (gameController) {
            gameController.currentBoat = this;
        }

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Stop();
            }
        }
    
        if (speedParticle)
            speedParticle.Stop();
        defaultPosition = transform.position.y;

        this.joyStickMove = FindObjectOfType<JoyStickMove>();
        this.joyStickMove.onMoveStart += this.onMoveStart;
        this.joyStickMove.onMoving += this.onMoving;
        this.joyStickMove.onMoveEnd += this.onMoveEnd;
    }
    public void onMoveStart()
    {

    }
    public void onMoving(Vector2 vector2)
    {
        this.detailMove = new Vector3(vector2.x, 0, 0);

    }

    public void onMoveEnd()
    {
        this.detailMove = Vector2.zero;

    }
    //Start the game
    public void StartTheGame() {        if (startRipple)
            startRipple.Stop();
        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Play();
            }
        }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WSAPlayerARM || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerX86)
        {
            switch (PlayerPrefs.GetInt("ControlType", 0)) {
            
                case 0:
                    controlType = ControlTypes.Accelerometer;
                    break;
                case 1:
                    controlType = ControlTypes.TapControls;
                    break;
            }
        }


        myAnimator = transform.GetComponent<Animator>();

        if (shieldGraphic)
        {
            shieldGraphic.SetActive(false);
        }

        if (speedParticle)
        {
            speedParticle.Stop();
        }

        isStarted = true;

        if (speedManager)
        {
            speedManager.enabled = true;
        }
    }

    // 每帧根据输入更新船体运动与得分
    void Update() {
        //Move according to the wave variables
        if (isOver || !isStarted) {
            if (graphic) {
                graphic.position = new Vector3(graphic.position.x,graphicsPosition.y + Mathf.Sin(Time.time * waveFrequency) * waveHeight, graphic.position.z);

            }

            return;
        }
        this.transform.Translate(this.detailMove * Time.deltaTime * moveSpeed, Space.World);
        // 依据操控方式读取水平输入
        switch (controlType) {
            case ControlTypes.Keyboard: horizontalInput = Input.GetAxis("Horizontal"); break;
            case ControlTypes.Accelerometer: horizontalInput = Mathf.Clamp(Input.acceleration.x * 3.0f, -1.0f, 1.0f); break;
            case ControlTypes.TapControls: horizontalInput = TapControlValue(horizontalInput); break;
        }
        //move graphic
        if (graphic)
        {
            graphic.position = new Vector3(graphic.position.x, graphic.position.y + Mathf.Sin((Time.time * waveFrequency)*Time.timeScale) * waveHeight, graphic.position.z);
            transform.eulerAngles = new Vector3(0.0f, Mathf.LerpAngle(transform.eulerAngles.y, maxTurnAngle * horizontalInput, turnSpeed * Time.deltaTime), -20.0f * horizontalInput);
        }
        //rotate rotors
        foreach (Transform rotor in rotors)
        {
            if (rotor)
            {
                rotor.Rotate(Vector3.forward * rotorSpeed);
            }
        }

        // 随时间提升速度并累计得分
        if (!speedManager)
        {
            Debug.LogWarning("Speed Manager not assigned");
        }

        if (speedManager.holdUpSpeed <= speedManager.maxSpeed)
            speedManager.holdUpSpeed += Time.deltaTime * 0.16f;

        speedManager.speed = speedManager.holdUpSpeed - (Mathf.Abs(horizontalInput) * 5.0f);

        score += speedManager.speed * Time.deltaTime;

        if (scoreText) {
            scoreText.text = "�÷� : " + score.ToString("F0"); 
        }

        if (speedManager.speed < 0.0f) {
            speedManager.speed = 0.0f;
        }
    }
    // 点触操控：左半屏左转，右半屏右转
    float TapControlValue(float oldInput) {
        float xAxis = 0.0f;

        Rect left = new Rect(0, 0, Screen.width / 2.0f, Screen.height);
        Rect right = new Rect(Screen.width / 2.0f, 0, Screen.width / 2.0f, Screen.height);

        foreach (Touch currentTouch in Input.touches) {
            if (left.Contains(currentTouch.position)) {
                xAxis = -1.0f;
                break;
            }
            if (right.Contains(currentTouch.position)){
                xAxis = 1.0f;
                break;
            }

        }

        return Mathf.Lerp(oldInput, xAxis, Time.deltaTime * 5.0f);
        
    }
    // 处理拾取道具与碰撞判定
    void OnTriggerEnter(Collider other) {

        if (isOver)
            return;

        if (other.GetComponent<PowerUps>())
        {
            other.GetComponent<PowerUps>().InitializePowerUp(transform);

            return;
        }else if (isProtected)
        {
            isProtected = false;
            other.enabled = false;
            if (shieldGraphic)
            {
                shieldGraphic.SetActive(false);
            }

            if (audioPlayer && shieldOffSFX)
            {
                audioPlayer.PlayOneShot(shieldOffSFX);
            }

            StopCoroutine(ShieldTimer());
            return;
        }
        
        Debug.Log("Over");

        if (crashedSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(crashedSFX);
            if (speedManager)
                speedManager.Crashed();
            
        }

        Invoke("Over", 1.0f);
        isOver = true;

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Stop();
            }
        }

        if (myAnimator) {
            myAnimator.enabled = true;
        }
        Time.timeScale = 1.0f;

        if (magnetGraphic)
        {
            magnetGraphic.SetActive(false);
        }

        StopCoroutine(Magnet());
        if (myAnimator)
            myAnimator.SetTrigger("Over");

        if (speedManager){
            speedManager.enabled = false;
        }
    }
    public void Respawn()
    {
        isOver = false;
        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Play();
            }
        }
        if (myAnimator)
        {
            myAnimator.enabled = false;
        }
        Time.timeScale = 1.0f;
        if (magnetGraphic)
        {
            magnetGraphic.SetActive(true);
        }
        if (speedManager)
        {
            speedManager.enabled = true;
        }
    }
    void Over() {
        if (!gameController) {
            return;
        }
        gameController.SetCoinValue(coins);
        gameController.EnableDisableEndPanel("Over" , score , coins);
        
    }
    // 开启护盾
    public void EnableShield() {        if (shieldSFX && audioPlayer) {
            audioPlayer.PlayOneShot(shieldSFX);
        }

        StopCoroutine(ShieldTimer());
        isProtected = true;
        if (shieldGraphic)
        {
            shieldGraphic.SetActive(true);
        }
        StartCoroutine(ShieldTimer());

    }

    IEnumerator ShieldTimer() {
        yield return new WaitForSeconds(shieldDuration);
        isProtected = false;
        if (shieldGraphic) {
            shieldGraphic.SetActive(false);
        }
        yield return null;
    }

    public void FillUpGas() {
        Time.timeScale = 1.0f;
        StopCoroutine(SpeedUp());
        StartCoroutine(SpeedUp());
    }
    // 加速道具：短时间提升游戏速度并附带护盾
    IEnumerator SpeedUp()
    {
        if (gasSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(gasSFX);
        }

        if (!speedParticle)
            yield return null;
        EnableShield();
        Time.timeScale = 1.5f;
        speedParticle.Play();
        yield return new WaitForSeconds(gasDuration);
        speedParticle.Stop();
        Time.timeScale = 1.0f;
        yield return null;
    }

    // 拾取金币
    public void AddCoin() {
        if (audioPlayer && coinSFX) {
            audioPlayer.PlayOneShot(coinSFX,0.25f);
        }

        coins++;
        if (coinText)
            coinText.text = "X " + coins.ToString();
    }
    // 拾取宝石
    public void AddGem()
    {
        if (gemSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(gemSFX);
        }
        PlayerPrefs.SetInt("Gems", PlayerPrefs.GetInt("Gems", 0)+1);

        gems++;
        if (gemText)
            gemText.text = "X " + gems.ToString();
    }
    // 开启磁铁
    public void EnableMagnet() {
        StopCoroutine(Magnet());
        StartCoroutine(Magnet());
    }

    IEnumerator Magnet() {
        if (magnetSFX && audioPlayer)
        {
            audioPlayer.PlayOneShot(magnetSFX);
        }

        if (magnetGraphic) {
            magnetGraphic.SetActive(true);
        }
        lastMagnetTime = Time.time;
        Collider[] allColliders;
        while ((Time.time - lastMagnetTime) < magnetDuration) {
            allColliders = Physics.OverlapSphere(transform.position, magnetRange);
            foreach (Collider current in allColliders) {
                if (current.isTrigger) {
                    current.SendMessage("SetCoinMagnet", transform,SendMessageOptions.DontRequireReceiver);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        if (magnetGraphic)
        {
            magnetGraphic.SetActive(false);
        }

        yield return null;
    }
    // 复活船体
    public void Revive() {
        if (!myAnimator) {
            return;
        }

        if (audioPlayer && reviveSFX)
        {
            audioPlayer.PlayOneShot(reviveSFX);
        }

        myAnimator.SetTrigger("Revive");

        StartCoroutine(ReviveRoutine());
    }

    IEnumerator ReviveRoutine() {
        yield return new WaitForSeconds(2.0f);

        EnableShield();
        if (speedManager)
        {
            speedManager.enabled = true;
        }

        foreach (ParticleSystem part in floatparticles)
        {
            if (part)
            {
                part.Play();
            }
        }
        isOver = false;
        yield return null;
    }

    // 在编辑器里画出磁铁吸附范围
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}
