using NetMQ;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
  
    private bool _initialized = false;
    public bool landscape = true;
    public RawImage image;
    public CanvasScaler scaler;
    public HandParams handParams;

    public Receiver receiver;
    public Sender sender;
    public Hand leftHand, rightHand;
    //public Hand_rotations leftRigidHand, rightRigidHand;
    public Texture2D receiveTexture;
    public Texture2D sendTexture;
    public RenderTexture renderTexture;
    public static float frustumHeight;
    public static float frustumWidth;

    public Data dataTransfer;
    public RigidHand leftRigidHand;

    private void Start()
    {
        var canvas = image.GetComponentInParent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        scaler = image.GetComponentInParent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        receiver = new Receiver();
        receiver.Start();

        sender = new Sender();
        sender.Start();
    }

    private void Update()
    {
        image.SetNativeSize();
        scaler.matchWidthOrHeight = landscape ? 1 : 0;
        scaler.referenceResolution = image.rectTransform.sizeDelta;

        while (receiver.toEventLoop.TryDequeue(out var dataAndFrame))
        {
            (Data data, byte[] frame) = dataAndFrame;
            dataTransfer = data;

            if (!_initialized)
            {
                var frameWidth = data.frameWidth;
                var frameHeight = data.frameHeight;

                frustumHeight = Camera.main.orthographicSize * 2;
                frustumWidth = frustumHeight * frameWidth / frameHeight;

                leftHand = new Hand(handParams, xMult: -1);
                rightHand = new Hand(handParams, xMult: 1);

  //              leftRigidHand = new Hand_rotations();
   //             rightRigidHand = new Hand_rotations();

                receiveTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, mipChain: false);
                sendTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, mipChain: false);
                image.texture = receiveTexture;

                _initialized = true;
            }
            
            receiveTexture.LoadRawTextureData(frame);
            receiveTexture.Apply(updateMipmaps: false); // image.texture = receiveTexture;

            leftHand.Process(data.dataL);
            rightHand.Process(data.dataR);
            leftRigidHand.Process(data.dataR);
      //      rightRigidHand.Process(data.dataR);

        }
    }

    private void OnDestroy()
    {
        receiver.Stop();
        sender.Stop();
        NetMQConfig.Cleanup();
    }

    private void OnPreRender()
    {
        if (!_initialized) return;
        Camera.main.targetTexture = renderTexture;
    }

    private void OnPostRender()
    {
        if (!_initialized) return;
        sendTexture.ReadPixels(new Rect(0, 0, sendTexture.width, sendTexture.height), 0, 0);
        sendTexture.Apply();
        while (!sender.fromEventLoop.IsEmpty) sender.fromEventLoop.TryDequeue(out _);
        sender.fromEventLoop.Enqueue(sendTexture.GetRawTextureData());
        Camera.main.targetTexture = null;
    }
}