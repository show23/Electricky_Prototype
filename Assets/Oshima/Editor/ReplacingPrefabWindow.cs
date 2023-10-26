using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Prefab�������ւ���E�B���h�E
/// </summary>
public class ReplacingPrefabWindow : EditorWindow
{

    //�X�N���[���ʒu
    private Vector2 _scrollPosition = Vector2.zero;

    //�����ւ����Prefab
    private GameObject _taretPrefab;

    //�e�l���Œ肷�邩
    private bool _isFixPosition = true, _isFixRotation = true, _isFixScale = true;

    //Undo���̖��O
    private const string UndoName = " Prefab�������ւ�";

    //=================================================================================
    //������
    //=================================================================================

    //���j���[����E�B���h�E��\��
    [MenuItem("Tools/Open/ReplacingPrefabWindow")]
    public static void Open()
    {
        ReplacingPrefabWindow.GetWindow(typeof(ReplacingPrefabWindow));
    }

    private void OnEnable()
    {
        //�I������Ă��镨���ς������GUI���X�V����悤��
        Selection.selectionChanged += Repaint;
    }

    //=================================================================================
    //�\������GUI�̐ݒ�
    //=================================================================================

    private void OnGUI()
    {
        //�X�N���[���ʒu�ۑ�
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUI.skin.scrollView);

        //�����ւ����Prefab�I��UI
        EditorGUILayout.LabelField("�����ւ����Prefab");
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        _taretPrefab = (GameObject)EditorGUILayout.ObjectField(_taretPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //�e�l���Œ肷�邩��ݒ肷��UI
        EditorGUILayout.LabelField("�����ւ����̌Œ肷�鍀��");
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        _isFixPosition = EditorGUILayout.ToggleLeft("Position", _isFixPosition, GUILayout.Width(100));
        _isFixRotation = EditorGUILayout.ToggleLeft("Rotation", _isFixRotation, GUILayout.Width(100));
        _isFixScale = EditorGUILayout.ToggleLeft("Scale", _isFixScale);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //�I��(�����ւ���)�̃I�u�W�F�N�g�ꗗ
        EditorGUILayout.LabelField("�����ւ���(�I�����Ă���)�I�u�W�F�N�g�ꗗ");
        EditorGUILayout.BeginVertical(GUI.skin.box);
        var selectingTransforms = Selection.transforms;
        foreach (var selectingTransform in selectingTransforms)
        {
            EditorGUILayout.LabelField($"{selectingTransform.name}");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();

        //�����ւ����s�{�^��
        if (_taretPrefab == null)
        {
            EditorGUILayout.HelpBox("�����ւ����Prefab���ݒ肳��Ă��܂���", MessageType.Error);
        }
        else if (selectingTransforms.Length == 0)
        {
            EditorGUILayout.HelpBox("�����ւ���I�u�W�F�N�g���I������Ă��܂���", MessageType.Error);
        }
        else if (GUILayout.Button("�����ւ����s"))
        {
            Replace(selectingTransforms);
        }

        //�`��͈͂�����Ȃ���΃X�N���[��
        EditorGUILayout.EndScrollView();
    }

    //=================================================================================
    //�����ւ�
    //=================================================================================

    private void Replace(Transform[] transforms)
    {
        foreach (var transform in transforms)
        {
            //���̃I�u�W�F�N�g�̈ʒu��Prefab�̂܂ܐݒu
            var newObject = (PrefabUtility.InstantiatePrefab(_taretPrefab) as GameObject).transform;
            Undo.RegisterCreatedObjectUndo(newObject.gameObject, UndoName);
            Undo.SetTransformParent(newObject, transform.parent, UndoName);
            newObject.SetSiblingIndex(transform.GetSiblingIndex());
            if (_isFixPosition)
            {
                newObject.position = transform.position;
            }
            if (_isFixRotation)
            {
                newObject.rotation = transform.rotation;
            }
            if (_isFixScale)
            {
                newObject.localScale = transform.localScale;
            }

            //���̃I�u�W�F�N�g��Undo�ɓo�^���č폜
            Undo.DestroyObjectImmediate(transform.gameObject);
        }
    }

}