using UnityEngine;

/// <summary>
/// In this class we can play specific animations, limit player movement when animations are playing, and adjust animator values and animations based on the movement input.
/// </summary>
public class AnimatorManager : MonoBehaviour
{
    // TO-DO: Create setter and getter for the animator component.
    // Animator of the player.
    [Tooltip("Animator of the player.")]
    [SerializeField]
    public Animator animator;
    // Horizontal parameter of the animator.
    int horizontal;
    // Vertical parameter of the animator.
    int vertical;

    /// <summary>
    /// When script is loaded initialize horizontal and vertical parameters of the animator.
    /// </summary>
    private void Awake()
    {
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    /// <summary>
    /// Method to play a selected animation. 
    /// Additionaly can set player status to interacting to limit movement during some animations.
    /// </summary>
    /// <param name="targetAnimation"> Animation to be played. </param>
    /// <param name="isInteracting"> Boolean indicating that player is occupied during the animation and cannot move. </param>
    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0f);
    }

    /// <summary>
    /// Method that updates animator horizontal and vertical values based on the input.
    /// </summary>
    /// <param name="horizontalMovement"> Horizontal movement input. </param>
    /// <param name="verticalMovement"> Vertical movement input. </param>
    /// <param name="isSprinting"> Boolean indicating that the player is sprinting. </param>
    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        // Adjusted horizontal movement input.
        float snappedHorizontal;
        // Adjusted vertical movement input.
        float snappedVertical;

        // Adjust recieved horizontal input to smooth animations.
        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }
        #endregion

        // Adjust recieved vertical input to smooth animations.
        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVertical = 1;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }
        #endregion

        // Check if the player is sprinting and adjust horizontal movement accordingly.
        if (isSprinting)
        {
            snappedHorizontal = horizontalMovement;
            snappedVertical = 2;
        }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
}
