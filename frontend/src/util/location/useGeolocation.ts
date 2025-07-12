import { useEffect, useState } from "react";

export type Position = {
    latitude: number;
    longitude: number;
    accuracy?: number;
};

export const useGeolocation = () => {
    const [position, setPosition] = useState<Position | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!navigator.geolocation) {
            setError("Geolocation wird von deinem Browser nicht unterstützt.");
            setLoading(false);
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (pos) => {
                const { latitude, longitude, accuracy } = pos.coords;
                setPosition({ latitude, longitude, accuracy });
                setLoading(false);
            },
            (err) => {
                setError(err.message);
                setLoading(false);
            }
        );
    }, []);

    return { position, error, loading };
};
