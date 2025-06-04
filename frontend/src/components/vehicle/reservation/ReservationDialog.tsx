import React from "react";
import {
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Typography,
} from "@mui/material";
import DefaultButton from "../../button/DefaultButton.tsx";

interface Props {
    open: boolean;
    onClose: () => void;
    onConfirm: () => void;
    model: string | null;
}

const ReservationDialog: React.FC<Props> = ({
    open,
    onClose,
    onConfirm,
    model,
}) => {
    return (
        <Dialog open={open} onClose={onClose}>
            <DialogTitle>Reserve Vehicle</DialogTitle>
            <DialogContent>
                <Typography>
                    Would you like to reserve the vehicle{" "}
                    <strong>{model}</strong>?
                </Typography>
            </DialogContent>
            <DialogActions>
                <DefaultButton onClick={onClose} color="secondary">
                    No
                </DefaultButton>
                <DefaultButton
                    onClick={onConfirm}
                    color="primary"
                    variant="contained"
                >
                    Yes
                </DefaultButton>
            </DialogActions>
        </Dialog>
    );
};

export default ReservationDialog;
