using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TouchEventInfo
{
    public Vector2 deltaPosition;
    public float deltaTime;
    public int fingerId;
    public TouchPhase phase;
    public Vector2 position;
    public Vector2 rawPosition;
    public int tapCount;
}

public class TouchInput : MonoBehaviour 
{
	public event TouchBeganDelegate TouchBegan;
	public event TouchMovedDelegate TouchMoved;
	public event TouchEndedDelegate TouchEnded;
	public event TapDelegate Tap;
	public event SwipeDelegate Swipe;
	public event PinchDelegate Pinch;

    public delegate void TouchBeganDelegate(TouchEventInfo touchInfo);
    public delegate void TouchMovedDelegate(TouchEventInfo touchInfo);
    public delegate void TouchEndedDelegate(TouchEventInfo touchInfo);
    public delegate void TapDelegate(TouchEventInfo touchInfo, float deltaTime);
    public delegate void SwipeDelegate(TouchEventInfo touchInfo, float deltaTime, Vector2 deltaPosition);
    public delegate void PinchDelegate(TouchEventInfo touchInfo1, Touch touchInfo2, float deltaTime, float deltaLength);
	
	public float gestureTime = 0.3f;
	
	private float tapDistanceThreshold = 0.025f;
	
	//the length between two fingers pinching
	private float lastPinchLength = 0;
	private Vector2 pinchPosition1 = Vector2.zero;
	private Vector2 pinchPosition2 = Vector2.zero;
	private Touch pinch1Touch;
	private float pinchSensitivity = 0.05f;

    



    //-- mouse input to simulate touches
    public struct MouseButtonState
    {
        public bool mouseButtonDown;
        public float startTime;
    }
    MouseButtonState m_MouseButtonState;
    bool isMouseButtonDown = false;
	
	private Dictionary<int, StartInformation> m_StartPositions = new Dictionary<int, StartInformation>();
	
	//class to store touch start info to derive deltas
	private class StartInformation
	{
		public Vector2 position;
		public float time;
	}
	
	void SetStartInformation(int fingerID, Vector2 startPosition, float startTime)
	{
		StartInformation start = new StartInformation();
		start.position = startPosition;
		start.time = startTime;
		
		if(m_StartPositions.ContainsKey(fingerID))
		{
			m_StartPositions[fingerID] = start;
		}
		else
		{
			m_StartPositions.Add(fingerID, start);
		}
	}
	
	StartInformation GetStartInformation(int fingerID)
	{
		if(m_StartPositions.ContainsKey(fingerID))
		{
			return m_StartPositions[fingerID];
		}
		else
		{
			return new StartInformation();
		}
	}
	
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		int numTouches = Input.touches.Length;

        if (numTouches > 0)
        {
            for (int i = 0; i < numTouches; i++)
            {
                //pinch needs two touches
                if (numTouches == 2)
                {
                    if (i == 0)
                    {
                        //record the first point
                        pinchPosition1 = Input.touches[i].position;
                        pinch1Touch = Input.touches[i];
                    }
                    else
                    {
                        //record the second point
                        pinchPosition2 = Input.touches[i].position;

                        //call the pinch event
                        if (Pinch != null)
                        {
                            float length = 0;
                            if (lastPinchLength != 0)
                            {
                                //length = Vector2.Distance(pinchPosition1, pinchPosition2) - lastPinchLength;
                                //calculate the normalized distance between both fingers
                                length = Vector2.Distance(normalizeTouchPosition(pinchPosition1), normalizeTouchPosition(pinchPosition2)) - lastPinchLength;
                            }

                            lastPinchLength = Vector2.Distance(normalizeTouchPosition(pinchPosition1), normalizeTouchPosition(pinchPosition2));


                            if (length > pinchSensitivity || length < -pinchSensitivity)
                            {
                                Pinch( convertTouchInfo(Input.touches[i]) , pinch1Touch, Time.time - GetStartInformation(Input.touches[i].fingerId).time, length);
                            }
                        }
                    }
                }


                //touch started
                if (Input.touches[i].phase == TouchPhase.Began)
                {

                    DebugOut.Instance.AddDebug("BEGAN FIRED");

                    //store the start position so there is a delta in the touchphase ended
                    SetStartInformation(Input.touches[i].fingerId, Input.touches[i].position, Time.time);

                    //*************************Event Touch Began
                    if (TouchBegan != null)
                    {
                        TouchBegan( convertTouchInfo(Input.touches[i]) );
                    }
                }

                //touch moved
                if (Input.touches[i].phase == TouchPhase.Moved)
                {
                    //*************************Event Touch Moved
                    if (TouchMoved != null)
                    {
                        TouchMoved( convertTouchInfo(Input.touches[i]) );
                    }
                }

                //touch ended
                if (Input.touches[i].phase == TouchPhase.Ended)
                {
                    //reset pinch
                    if (numTouches == 2)
                    {
                        lastPinchLength = 0;
                        pinchPosition1 = Vector2.zero;
                        pinchPosition2 = Vector2.zero;
                    }

                    //*************************Event TouchEnded
                    if (TouchEnded != null)
                    {
                        TouchEnded( convertTouchInfo(Input.touches[i]) );
                    }

                    //if touch ended within a threshold time it is a swipe or a tap					
                    if (Time.time - GetStartInformation(Input.touches[i].fingerId).time < gestureTime)
                    {

                        //DebugOut.Instance.AddDebug("GESTURES DISTANCE: " + Vector2.Distance(Input.touches[i].position, GetStartInformation(Input.touches[i].fingerId).position).ToString());

                        //hasn't moved past threshhold, so it's a tap

                        //normalize touch position
                        Vector2 touchPos = normalizeTouchPosition(Input.touches[i].position);
                        //normalize start information position
                        Vector2 startPos = normalizeTouchPosition(GetStartInformation(Input.touches[i].fingerId).position);

                        //float dist = Vector2.Distance(Input.touches[i].position, GetStartInformation(Input.touches[i].fingerId).position);
                        float dist = Vector2.Distance(touchPos, startPos);
                        DebugOut.Instance.AddDebug("Distance: " + dist);
                        if (dist < tapDistanceThreshold)
                        {
                            //*************************Event Tapped
                            if (Tap != null)
                            {
                                Tap( convertTouchInfo(Input.touches[i]), Time.time - GetStartInformation(Input.touches[i].fingerId).time);
                            }
                        }
                        //touch moved so it is a swipe
                        else
                        {
                            //*************************Event Swipe
                            if (Swipe != null)
                            {
                                Swipe( convertTouchInfo(Input.touches[i]), Time.time - GetStartInformation(Input.touches[i].fingerId).time, Input.touches[i].position - GetStartInformation(Input.touches[i].fingerId).position);
                                //m_TouchEventInfo.Remove(Input.GetTouch(i).fingerId);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //-- no touch, check mouse input            
            isMouseButtonDown = Input.GetMouseButton( 0 );

            //-- if the input of the mouse has changed then handle it
            if( isMouseButtonDown != m_MouseButtonState.mouseButtonDown )
            {
                //-- populate the mouse info with the mouse position
                TouchEventInfo mouseInfo = new TouchEventInfo();
                mouseInfo.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

                //-- began fired
                if (isMouseButtonDown == true)
                {
                    //-- cache the start time
                    m_MouseButtonState.startTime = Time.time;

                    //-- fire the began event
                    if( TouchBegan != null )
                    {
                        TouchBegan( mouseInfo );
                    }
                }
                //-- end fired
                else
                {
                    //-- if within the gesture threshhold value, then register as a tap
                    float delta = Time.time - m_MouseButtonState.startTime;
                    if( delta < gestureTime )
                    {
                        if( Tap != null )
                        {
                            Tap( mouseInfo, delta );
                        }
                    }

                    //-- fire ended event
                    if( TouchEnded != null )
                    {
                        TouchEnded( mouseInfo );
                    }
                }

                m_MouseButtonState.mouseButtonDown = isMouseButtonDown;
                
            }
        }
	}
	
	public Vector2 normalizeTouchPosition(Vector2 pos)
	{
		return new Vector2(pos.x / Screen.width, pos.y / Screen.height);
	}

    private TouchEventInfo convertTouchInfo( Touch info )
    {
        TouchEventInfo touchInfo;
        touchInfo.deltaPosition = info.deltaPosition;
        touchInfo.deltaTime = info.deltaTime;
        touchInfo.fingerId = info.fingerId;
        touchInfo.phase = info.phase;
        touchInfo.position = info.position;
        touchInfo.rawPosition = info.rawPosition;
        touchInfo.tapCount = info.tapCount;

        return touchInfo;
    }

}
