using NetMQ;
using UnityEngine;
using UnityEngine.UI;

public class VertClient : MonoBehaviour
{
  
    private bool _initialized = false;
    public bool landscape = true;
    public RawImage image;
    public CanvasScaler scaler;
    public HandParams handParams;

    public VertReceiver receiver;
    public Sender sender;
    public Hand leftHand, rightHand;
    public Texture2D receiveTexture;
    public Texture2D sendTexture;
    public RenderTexture renderTexture;
    public static float frustumHeight;
    public static float frustumWidth;

    public VertData dataTransfer;
    public VertRigidHand leftRigidHand;
    public Vector3 location;


    private void Start()
    {
        var canvas = image.GetComponentInParent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        scaler = image.GetComponentInParent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        receiver = new VertReceiver();
        receiver.Start();

        sender = new Sender();
        sender.Start();
        AsyncIO.ForceDotNet.Force();
    }

    private void Update()
    {
        image.SetNativeSize();
        scaler.matchWidthOrHeight = landscape ? 1 : 0;
        scaler.referenceResolution = image.rectTransform.sizeDelta;

        while (receiver.toEventLoop.TryDequeue(out var dataAndFrame))
        {
            (VertData data, byte[] frame) = dataAndFrame;
            dataTransfer = data;

            if (!_initialized)
            {
                var frameWidth = data.frameWidth;
                var frameHeight = data.frameHeight;

                frustumHeight = Camera.main.orthographicSize * 2;
                frustumWidth = frustumHeight * frameWidth / frameHeight;

                leftHand = new Hand(handParams, xMult: -1);
                rightHand = new Hand(handParams, xMult: 1);


                receiveTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, mipChain: false);
                sendTexture = new Texture2D(frameWidth, frameHeight, TextureFormat.RGB24, mipChain: false);
                image.texture = receiveTexture;

                _initialized = true;
            }
            
            receiveTexture.LoadRawTextureData(frame);
            receiveTexture.Apply(updateMipmaps: false); // image.texture = receiveTexture;
       //     location = CalLocation(data.datal);

            //leftHand.Process(data.dataL);
            //rightHand.Process(data.dataR);
            leftRigidHand.Process(data.left_hand_data);
      //      rightRigidHand.Process(data.dataR);


        }
    }

    private void OnDestroy()
    {
        receiver.Stop();
        sender.Stop();
        NetMQConfig.Cleanup(false);
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

    //private Vector3 CalLocation(VertData.HandData data)
    //{
     //   Debug.Log(data.distX);
        //float scaleX = data.vert * data.distX + (1 - data.vert) * data.distY;
        //float scaleY = data.vert * data.distY + (1 - data.vert) * data.distX;
        //float dist = (scaleX + scaleY) / 2;
        //var target = new Vector3(
        //    (+data.joints[0].x / scaleX + data.origin.x) * (VertClient.frustumWidth / 2) * (1),
        //    (-data.joints[0].y / scaleY + (data.origin.y - 0.5f)) * VertClient.frustumHeight,
        //    1f * data.joints[0].z + 1f * dist);
        //var actualTarget = Vector3.Lerp(.transform.position, target,
         //      Vector3.Distance(.transform.position, target) * 5f);
    //    var target = new Vector3(0, 0, 0);
   //     return target;
   // }
}