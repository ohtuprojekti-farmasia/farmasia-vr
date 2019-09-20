﻿using UnityEngine;
using UnityEngine.Assertions;

public class TeleportControls : MonoBehaviour {

    #region fields
    private TeleportArc arc;
    private LineRenderer line;

    private bool isTeleporting;
    private bool isInvalidTeleport;
    private Vector3 hitPos;

    [SerializeField]
    private Transform player;
    private Transform pHead;
    #endregion

    private void Start() {
        line = GetComponent<LineRenderer>();
        arc = new TeleportArc();
        Assert.IsNotNull(player, "Player transform has not been set in Editor");
        pHead = player.GetChild(2);
    }

    public void StartTeleport() {
        isTeleporting = true;
    }
    public void EndTeleport() {
        isTeleporting = false;
        line.positionCount = 0;
        TeleportToPosition();
    }

    private void Update() {
        if (isTeleporting) {
            DrawLine();
        }
    }

    private void DrawLine() {
        GameObject hit;
        Vector3[] positions;

        arc.ShootArc(transform.position, transform.forward, out positions, out hit, out hitPos);

        isInvalidTeleport = hit == null || hit.gameObject.tag != "Floor";

        Logger.PrintVariables("count", positions.Length);

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    private void TeleportToPosition() {
        if (isInvalidTeleport) {
            Logger.Print("Invalid teleport");
            return;
        }

        player.transform.position = hitPos - CameraOffset();
    }
    private Vector3 CameraOffset() {

        Vector3 offset = pHead.position - player.position;
        offset.y = 0;

        return offset;
    }
}