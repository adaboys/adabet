<?php

namespace App\Traits;

use Symfony\Component\HttpFoundation\Response as ResponseAlias;

trait ApiResponse
{
    public function httpCreated($status = 200, $message = 'created new user success full')
    {
        return response()->json([
            'status' => $status,
            'message' => $message,
            'data' => null
        ], ResponseAlias::HTTP_OK);
    }

    public function httpOk($message, $data = [], $status = 200)
    {
        return response()->json([
            'status' => $status,
            'message' => $message,
            'data' => $data,
            'code' => $status,
        ], ResponseAlias::HTTP_OK);
    }

}