using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

namespace EGF.Utilities
{
    public static class Helper
    {
        public const long NUMBER_1_TRILLION       = 1000000000000; //1,000,000,000,000;
        public const long NUMBER_1_BILLION        = 1000000000;    //1,000,000,000;
        public const long NUMBER_1_MILLION        = 1000000;       //1,000,000;
        public const long NUMBER_1_THOUSAND       = 1000;          //1,000;
        
        private static Tweener _TimeTweener;
        
        public static Tweener AnimateNumberForTextRoundUp(this TextMeshProUGUI txt, long from, long to, float duration, string prefix = "$", string suffix = "", TweenCallback onComplete = null)
        {
            return DOVirtual.Float(from, to, duration, value =>
            {
                txt.text = prefix + GetRoundUpNumbersAsString((long)value) + suffix;
            }).OnComplete(onComplete).SetLink(txt.gameObject);
        }

        public static long GetRoundUpNumbers(long number)
        {
            if (number < NUMBER_1_THOUSAND)
                return number;
            
            //Debug.Log("number: " + number);
            long num = NUMBER_1_THOUSAND;
            if (number >= NUMBER_1_TRILLION)
            {
                num = NUMBER_1_TRILLION;
            }
            if (number >= NUMBER_1_BILLION)
            {
                num = NUMBER_1_BILLION;
            }
            else if (number >= NUMBER_1_MILLION)
            {
                num = NUMBER_1_MILLION;
            }

            int d = Mathf.FloorToInt(number / num);
            
            //Debug.Log("d: " + d);
            long r = number % num;
            
            int t = Mathf.FloorToInt( Mathf.FloorToInt(r / (num * 0.1f)) * (num * 0.1f) );
            //Debug.Log("t: " + t);

            number = (long) (d * num + t);
            //Debug.Log("new: " + number);
            return number;
        }
        public static string GetRoundUpNumbersAsString(long number)
        {
            number = GetRoundUpNumbers(number);
            
            if (number < NUMBER_1_THOUSAND)
                return number.ToString();
            
            if (number >= NUMBER_1_TRILLION)
            {
                return ((number / (float)NUMBER_1_TRILLION)).ToString("0.00") + "T";
            }
            
            if (number >= NUMBER_1_TRILLION)
            {
                return ((number / (float)NUMBER_1_TRILLION)).ToString("0.00") + "T";
            }
            
            if (number >= NUMBER_1_BILLION)
            {
                return (number / (float)NUMBER_1_BILLION) + "B";
            }
            
            if (number >= NUMBER_1_MILLION)
            {
                return (number / (float)NUMBER_1_MILLION) + "M";
            }
            
            return (number / (float)NUMBER_1_THOUSAND) + "K";
        }

        public static int GetRoundUpBasedOnFirstFewDigits(int number, int firstDigitsLength)
        {
            string numberS = number.ToString();
            if (numberS.Length < firstDigitsLength) return number;

            string result = numberS.Substring(0, firstDigitsLength);
            StringBuilder sb = new StringBuilder(result);
            
            int rest = numberS.Length - firstDigitsLength;

            for (int i = 0; i < rest; i++)
            {
                sb.Append("0");
            }

            result = sb.ToString();
            
            return int.Parse(result);
        }
        
        public static int GetDigitsCount(int n)
        {
            return n == 0 ? 1 : (n > 0 ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));
        }
        


        public static void PauseTime(float duration, System.Action func = null)
        {
            _TimeTweener = DOVirtual.Float(
                        Time.timeScale, 
                        0, 
                        duration, 
                        value => Time.timeScale = value)
                    .SetUpdate(true)
                    .OnComplete(delegate
                    {
                        func?.Invoke();
                    })
                ;
        }

        public static void UnPauseTime()
        {
            if(_TimeTweener != null) _TimeTweener.Kill();
            Time.timeScale = 1;
        }

        public static string GetSentenceFromCamelCase(string camel)
        {
            camel = Regex.Replace(camel, "([a-z])([A-Z])", "$1$2");
            camel = Regex.Replace(camel, "([A-Z])([a-z])", " $1$2");
            return camel;
        }

        public static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            
            return new Rect(new Vector2(transform.position.x - size.x, transform.position.y), size);
        }
        
         public static bool IsTouchingUIElement(Vector2 pos, GameObject allowedObject = null)
        {
            // if(EventSystem.current.IsPointerOverGameObject())
            // {
            //     return true; // yes mouse is over some UI elevent
            // }

            // the above code doesn't work for touch. so we need to use raycast into ui element
            if(Input.touchCount > 0)
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position =  pos;
                List<RaycastResult> listRayCastResult = new List<RaycastResult>();
			
                EventSystem.current.RaycastAll(pointer, listRayCastResult);

                if (allowedObject != null)
                {
                    bool onlyTouchedAllowedObject = false;
                    foreach(RaycastResult rcr in listRayCastResult)
                    {
                        if (rcr.gameObject == allowedObject || rcr.gameObject.transform.IsChildOf(allowedObject.transform))
                        {
                            onlyTouchedAllowedObject = true;
                        }
                        else
                        {
                            //Debug.Log("Touched: " + rcr.gameObject.name);
                            return true;
                        }
                    }

                    if (onlyTouchedAllowedObject) return false;
                }
                else
                {
                    if(listRayCastResult.Count > 0) return true; // yes there was UI element
                }

                
            }

            return false;
        }

        public static bool IsTouchingUIElement(GameObject obj, Vector2 pos)
        {
            if(Input.touchCount > 0)
            {
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position =  pos;
                List<RaycastResult> listRayCastResult = new List<RaycastResult>();

                EventSystem.current.RaycastAll(pointer, listRayCastResult);
                
                if(listRayCastResult.Count == 0) return false; 

                foreach(RaycastResult rcr in listRayCastResult)
                {
                    if(rcr.gameObject == obj || rcr.gameObject.transform.IsChildOf(obj.transform))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        
        public static Tweener AnimateNumberForText(this TextMeshProUGUI txt, int from, int to, float duration, string prefix = "$", string suffix = "", TweenCallback onComplete = null)
        {
            return DOVirtual.Int(from, to, duration, value =>
            {
                txt.text = prefix + value + suffix;
            }).OnComplete(onComplete).SetLink(txt.gameObject);
        }
        
        public static Tweener DOFloat(this float _value, float from, float to, float duration, TweenCallback onComplete = null)
        {
            return DOVirtual.Float(from, to, duration, value =>
            {
                _value = value;
            }).OnComplete(onComplete);
        }
        
        public static T BinaryWeightedRandom<T>(T option1, T option2, double weightOption1, double weightOption2)
        {
            double totalWeight = weightOption1 + weightOption2;
            double randomNumber = new Random().NextDouble() * totalWeight;

            if (randomNumber < weightOption1)
            {
                return option1;
            }
            else
            {
                return option2;
            }
        }
    }
}