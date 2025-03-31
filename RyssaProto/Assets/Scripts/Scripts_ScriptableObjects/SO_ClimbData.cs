using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableObjectScript", menuName = "Scriptable Objects/NewScriptableObjectScript")]
public class ClimbData : ScriptableObject
{
    public float climbSpeed = 3f;
    public bool allow_Strafe_Movement=false;
    public bool useSamina = false;
    public float staminaDrainPerSecond = 1f;

}
