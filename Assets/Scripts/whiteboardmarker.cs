using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;


public class whiteboardmarker : MonoBehaviour
{
    [SerializeField] private Transform tip;
    [SerializeField] private int penSize = 5;

    private Renderer renderer;
    private Color[] colors;
    private float tipHeight;
    private RaycastHit touch;
    private Whiteboard whiteBoard;
    private Vector2 touchPos, lastTouchPos;
    private bool touchedLastFrame;
    private Quaternion lastTouchRot;

    

    // Start is called before the first frame update
    void Start()
    {
        renderer = tip.GetComponent<Renderer>();
        colors = Enumerable.Repeat(renderer.material.color, penSize * penSize).ToArray();
        tipHeight = tip.localScale.y; 
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    private void Draw()
    {

        if(Physics.Raycast(tip.position, transform.up, out touch, tipHeight))
        {

            if(touch.transform.CompareTag("whiteboard")) {

                if(whiteBoard==null)
                {
                    whiteBoard = touch.transform.GetComponent<Whiteboard>();
                }
                 
                touchPos = new Vector2(touch.textureCoord.x, touch.textureCoord.y);
                var x = (int)(touchPos.x * whiteBoard.textureSize.x - (penSize / 2));
                var y = (int)(touchPos.y * whiteBoard.textureSize.y - (penSize / 2));

                if (y < 0 || y > whiteBoard.textureSize.y || x < 0 || x > whiteBoard.textureSize.x)
                {
                    return;
                }

                if (touchedLastFrame)
                {
                    whiteBoard.texture.SetPixels(x, y, penSize, penSize, colors);

                    for (float f = 0.01f; f < 1.00f; f += 0.03f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchPos.y, y, f);
                        whiteBoard.texture.SetPixels(lerpX, lerpY, penSize, penSize, colors);
                    }

                    transform.rotation = lastTouchRot;
                    whiteBoard.texture.Apply();
                }


                lastTouchPos = new Vector2(x, y);
                lastTouchRot = transform.rotation;
                touchedLastFrame = true;
                return;
            }
        }
        whiteBoard = null;
        touchedLastFrame = false; 
    }
}
