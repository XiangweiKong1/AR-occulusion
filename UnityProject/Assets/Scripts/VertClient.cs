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

    public VertRigidHand leftRigidHand, rightRigidhand;
    private Vector3 leftLocation, rightLocation;
    private Vector3 projectedLeftLocation, projectedRightLocation;

    public Vector3 leftLocationOffset;
    public Vector3 rightLocationOffset;


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

            leftLocation = CalLocation(data.dataL, -1, handParams);
            projectedLeftLocation = Projection(leftLocation, frustumHeight, frustumWidth);

            rightLocation = CalLocation(data.dataR, 1, handParams);
            projectedRightLocation = Projection(rightLocation, frustumHeight, frustumWidth);


            leftRigidHand.Process(data.left_hand_data);
            projectedLeftLocation += leftLocationOffset;
            leftRigidHand.transform.position = projectedLeftLocation;
           

            rightRigidhand.Process(data.right_hand_data);
            projectedRightLocation += rightLocationOffset;
            rightRigidhand.transform.position = projectedRightLocation;


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

    private Vector3 CalLocation(VertData.HandData data, int i, HandParams handParams)
    {
        float scaleX = data.vert * data.distX + (1 - data.vert) * data.distY;
        float scaleY = data.vert * data.distY + (1 - data.vert) * data.distX;
        float dist = (scaleX + scaleY) / 2;
        var target = new Vector3(
            (+data.joints[0].x / scaleX + data.origin.x) * (VertClient.frustumWidth / 2) * (i),
            (-data.joints[0].y / scaleY + (data.origin.y - 0.5f)) * VertClient.frustumHeight,
            handParams.zAlpha * data.joints[0].z + handParams.zBeta * dist);
        var actualTarget = Vector3.Lerp(leftRigidHand.transform.position, target,
               Vector3.Distance(leftRigidHand.transform.position, target) * handParams.speed);
        //var target = new Vector3(0, 0, 0);
        return actualTarget;
    }

    private Vector3 Projection(Vector3 location, float frustumHeight, float frustumwight)
    {
        float z = location.z;
      
        var v = new Vector4(
            location.x / (frustumWidth/2), location.y / (frustumHeight/2), location.z, 1);

        Debug.Log((location.x, frustumWidth, location.y, frustumHeight));

        Matrix4x4 projectionMatrix = Camera.main.projectionMatrix;
        v = projectionMatrix.inverse * v;

        var projectedLocation = new Vector3(v.x * z, v.y * z, z);

        return projectedLocation;
    }
}