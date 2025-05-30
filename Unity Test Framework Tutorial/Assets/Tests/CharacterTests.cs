/*
    https://unity.com/how-to/automated-tests-unity-test-framework
*/

using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using StarterAssets;

public class CharacterTests : InputTestFixture
{
    // Stores a reference to the "Character" prefab, loaded from the Resources folder
    GameObject character = Resources.Load<GameObject>("Character");
    // Holds a reference to the Keyboard input device provided by InputSystem
    Keyboard keyboard;

    public override void Setup()
    {
        // Runs the base class Setup(), then sets up your own CharacterTests by loading the test scene and initializing the keyboard input device
        SceneManager.LoadScene("Scenes/SimpleTesting");
        base.Setup();
        keyboard = InputSystem.AddDevice<Keyboard>();

        // The mouse input is added purely for the Third Person Controller to begin to receive input from the simulated/virtual keyboard device. This is almost like a "set focus" action.
        var mouse = InputSystem.AddDevice<Mouse>();
        Press(mouse.rightButton);
        Release(mouse.rightButton);
    }

    // Instantiate the character from the prefab and assert that it is not null.
    [Test]
    public void TestPlayerInstantiation()
    {
        GameObject characterInstance = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);
        Assert.That(characterInstance, !Is.Null);
    }

    /*
        [Test] runs for a single frame, while [UnityTest] runs for multiple frames.
        Generally, you should use the NUnit Test attribute instead of the UnityTest attribute in Edit mode, unless you need to yield special instructions, need to skip a frame, or wait for a certain amount of time in Play mode.
    */

    [UnityTest]
    public IEnumerator TestPlayerMoves()
    {
        // Instantiates an instance of the character from the character prefab at location (0, 0, 0)
        GameObject characterInstance = GameObject.Instantiate(character, Vector3.zero, Quaternion.identity);

        // Presses the up arrow key on the virtual keyboard for 1 second, then releases it
        Press(keyboard.upArrowKey);
        yield return new WaitForSeconds(1f);
        Release(keyboard.upArrowKey);
        // Waits 1 more second (for the character to slow down and stop moving)
        yield return new WaitForSeconds(1f);

        // Asserts that the character has moved to a position on the Z axis greater than 1.5 units
        Assert.That(characterInstance.transform.GetChild(0).transform.position.z, Is.GreaterThan(1.5f));
    }

    /*
        Arrange, Act, Assert (AAA) pattern commonly found in testing:
        
        1) The Arrange section of a unit test method initializes objects and sets the value of the data that is passed to the method under test.

        2) The Act section invokes the method under test with the arranged parameters. In this case, invoking the method under test is handled by a physics interaction when the player hits the ground after falling.

        3) The Assert section verifies that the action of the method under test behaves as expected.
    */

    [UnityTest]
    public IEnumerator TestPlayerFallDamage()
    {
        // Spawn the character in a high-enough area in the test scene
        GameObject characterInstance = GameObject.Instantiate(character, new Vector3(0f, 4f, 17.2f), Quaternion.identity);

        // Get a reference to the PlayerHealth component and assert currently at full health (1f)
        var characterHealth = characterInstance.GetComponent<PlayerHealth>();
        Assert.That(characterHealth.Health, Is.EqualTo(1f));

        // Walk off the ledge and wait for the fall
        Press(keyboard.upArrowKey);
        yield return new WaitForSeconds(0.5f);
        Release(keyboard.upArrowKey);
        yield return new WaitForSeconds(2f);

        // Assert that 1 health point was lost due to the fall damage
        Assert.That(characterHealth.Health, Is.EqualTo(0.9f));
    }
}
