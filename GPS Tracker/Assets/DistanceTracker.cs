using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class DistanceTracker : MonoBehaviour
{
    public float curLat, curLon, oldLat, oldLong;

    private bool setInitialValues = true;
    private double distance = 0;

    private void Start()
    {
/*#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            dialog = new GameObject();
        }
#endif*/

        StartCoroutine(GetDistanceTravelled());
    }

    private IEnumerator GetDistanceTravelled()
    {
        yield return new WaitForSeconds(3f);

        while(true)
        {
            if(!Input.location.isEnabledByUser)
            {
                this.GetComponent<Text>().text = "Enable Location";
                yield break;
            }

            Input.location.Start(1f, 0.1f);

            int maxWaitTime = 20;
            while(Input.location.status == LocationServiceStatus.Initializing && maxWaitTime > 0)
            {
                yield return new WaitForSeconds(1f);
                this.GetComponent<Text>().text = "Initializing in " + maxWaitTime + " seconds";
                maxWaitTime--;
            }

            if(maxWaitTime < 1)
            {
                this.GetComponent<Text>().text = "Timed out";
                yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                this.GetComponent<Text>().text = "Unable to determine device location";
                yield break;
            }
            else
            {
                if (setInitialValues)
                {
                    oldLat = Input.location.lastData.latitude;
                    oldLong = Input.location.lastData.longitude;
                    setInitialValues = false;
                }

                curLat = Input.location.lastData.latitude;
                curLon = Input.location.lastData.longitude;

                CalculateDistance(oldLat, oldLong, curLat, curLon);
            }

            Input.location.Stop();
        }
    }

    private void CalculateDistance(float lat1, float long1, float lat2, float long2)
    {
        float radius = 6378.137f;
        float dLat = (lat2 * Mathf.PI / 180) - (lat1 * Mathf.PI / 180);
        float dLong = (long2 * Mathf.PI / 180) - (long1 * Mathf.PI / 180);
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
            Mathf.Cos(lat1 * Mathf.PI / 180) * Mathf.Cos(lat2 * Mathf.PI / 180) *
            Mathf.Sin(dLong / 2) * Mathf.Sin(dLong / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        distance += radius * c * 1000;
        this.GetComponent<Text>().text = "Distance: " + distance.ToString() + "m\n" + lat1 + "\n" + lat2 + "\n" + long1 + "\n" + long2;
    }
}