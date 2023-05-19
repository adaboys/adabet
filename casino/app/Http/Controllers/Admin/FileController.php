<?php
/**
 *   Stake ADABET platform
 *   ----------------------
 *
 *   @copyright  Copyright (c) ADABET, All rights reserved
 *   @author     dev <contact@adabet.io>
 *   @see        https://adabet.io
 */

namespace App\Http\Controllers\Admin;

use Illuminate\Http\Request;
use App\Http\Controllers\Controller;
use Illuminate\Support\Facades\Storage;

class FileController extends Controller
{
    public function store(Request $request)
    {
        try {
            
            $path = $request->file->storeAs(
                $request->folder,
                $request->name . '-' . time() . '.' . $request->file->getClientOriginalExtension(),
                'public'
            );

            return $this->successResponse(['path' => $path ? '/storage/' . $path : '']);
        } catch (\Throwable $e) {
            return $this->errorResponse($e->getMessage());
        }
    }

    public function update(Request $request, string $name)
    {
        try {
            
            Storage::disk('public')->put('html/' . $name . '.html', $request->get('content'));
            return $this->successResponse();
        } catch (\Throwable $e) {
            return $this->errorResponse($e->getMessage());
        }
    }
}
