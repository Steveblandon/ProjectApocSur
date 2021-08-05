using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The purpose of this class is to make sure that both <see cref="Character"/> and it's assigned 
/// <see cref="SelectablePortrait"/> get selected and deselected together.
/// </summary>
public class CharacterSelector : MonoBehaviour
{
    private readonly ConcurrentDictionary<int, Character> selected = new ConcurrentDictionary<int, Character>();

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

                if (hit.collider != null)
                {
                    Character character = hit.collider.GetComponent<Character>();
                    if (character != null)
                    {
                        this.Select(character);

                        if (character.Portrait != null)
                        {
                            EventSystem.current.SetSelectedGameObject(character.Portrait.gameObject);
                        }
                        else
                        {
                            Debug.Log($"unable to select portrait for {character}:{character.GetInstanceID()}. Missing portrait assignment");
                        }
                    }
                }
                else
                {
                    foreach (var entry in this.selected)
                    {
                        this.Deselect(entry.Value);
                    }
                }
            }
            else if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out SelectablePortrait portrait)
                && portrait.Character != null)
            {
                this.Select(portrait.Character);
            }
        }
    }

    public void Select(Character character)
    {
        if (!this.selected.ContainsKey(character.GetInstanceID()))
        {
            Utils.ExecuteWithRetries(() => this.selected.TryAdd(character.GetInstanceID(), character));
            if (!this.selected.ContainsKey(character.GetInstanceID()))
            {
                Debug.Log($"unable to add {character}:{character.GetInstanceID()} as selected by ThingSelectionManager");
            }
            else
            {
                character.OnSelect();
            }
        }
    }

    public void Deselect(Character character)
    {
        if (this.selected.ContainsKey(character.GetInstanceID()))
        {
            Utils.ExecuteWithRetries(()=>this.selected.TryRemove(character.GetInstanceID(), out Character removedCharacter));
            if (this.selected.ContainsKey(character.GetInstanceID()))
            {
                Debug.Log($"unable to remove {character}:{character.GetInstanceID()} as selected by ThingSelectionManager");
            }
            else
            {
                character.OnDeselect();
            }
        }
    }
}
